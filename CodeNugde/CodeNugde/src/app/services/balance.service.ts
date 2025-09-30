import { Injectable, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, BehaviorSubject, map, tap } from 'rxjs';
import {
  UserBalance,
  Transaction,
  TransactionType,
  TransactionStatus,
  PaymentMethod,
  PaymentRequest,
  PaymentResponse,
  SubscriptionPlan,
  UserSubscription,
  CreditPackage,
  BillingHistory,
  BalanceAnalytics,
  InterviewCost,
  BalanceNotificationSettings
} from '../models/balance.model';

// Service for managing user balance, payments, and subscriptions
@Injectable({
  providedIn: 'root'
})
export class BalanceService {
  private readonly API_URL = '/api/balance';
  
  // Current user balance state
  private userBalanceSubject = new BehaviorSubject<UserBalance | null>(null);
  public userBalance$ = this.userBalanceSubject.asObservable();
  
  // Reactive signals for balance state
  currentBalance = signal<number>(0);
  subscriptionStatus = signal<string>('none');
  isLoading = signal<boolean>(false);
  
  // Cache for frequently accessed data
  private subscriptionPlansCache = new BehaviorSubject<SubscriptionPlan[]>([]);
  public subscriptionPlans$ = this.subscriptionPlansCache.asObservable();
  
  private creditPackagesCache = new BehaviorSubject<CreditPackage[]>([]);
  public creditPackages$ = this.creditPackagesCache.asObservable();

  constructor(private http: HttpClient) {
    this.loadUserBalance();
  }

  // Get current user balance
  getUserBalance(): Observable<UserBalance> {
    return this.http.get<UserBalance>(`${this.API_URL}/user`)
      .pipe(
        tap(balance => {
          this.userBalanceSubject.next(balance);
          this.currentBalance.set(balance.currentBalance);
          this.subscriptionStatus.set(balance.subscriptionStatus);
        })
      );
  }

  // Load user balance on service initialization
  private loadUserBalance(): void {
    this.getUserBalance().subscribe({
      next: (balance) => {
        // Balance loaded successfully
      },
      error: (error) => {
        console.error('Failed to load user balance:', error);
      }
    });
  }

  // Get transaction history
  getTransactionHistory(limit?: number, offset?: number): Observable<Transaction[]> {
    let params = new HttpParams();
    if (limit) params = params.set('limit', limit.toString());
    if (offset) params = params.set('offset', offset.toString());

    return this.http.get<Transaction[]>(`${this.API_URL}/transactions`, { params });
  }

  // Get specific transaction details
  getTransaction(transactionId: string): Observable<Transaction> {
    return this.http.get<Transaction>(`${this.API_URL}/transactions/${transactionId}`);
  }

  // Purchase credits
  purchaseCredits(packageId: string, paymentMethodId: string): Observable<PaymentResponse> {
    const request: PaymentRequest = {
      amount: 0, // Will be set by backend based on package
      currency: 'USD',
      paymentMethodId,
      description: `Credit package purchase: ${packageId}`,
      metadata: { packageId, type: 'credit_purchase' }
    };

    return this.http.post<PaymentResponse>(`${this.API_URL}/purchase-credits`, request)
      .pipe(
        tap(response => {
          if (response.success) {
            this.refreshUserBalance();
          }
        })
      );
  }

  // Subscribe to a plan
  subscribeToPlan(planId: string, paymentMethodId: string): Observable<PaymentResponse> {
    const request = {
      planId,
      paymentMethodId
    };

    return this.http.post<PaymentResponse>(`${this.API_URL}/subscribe`, request)
      .pipe(
        tap(response => {
          if (response.success) {
            this.refreshUserBalance();
          }
        })
      );
  }

  // Cancel subscription
  cancelSubscription(subscriptionId: string, reason?: string): Observable<{ success: boolean }> {
    const request = { reason };
    return this.http.post<{ success: boolean }>(`${this.API_URL}/subscriptions/${subscriptionId}/cancel`, request)
      .pipe(
        tap(response => {
          if (response.success) {
            this.refreshUserBalance();
          }
        })
      );
  }

  // Check if user has sufficient balance for interview
  checkInterviewBalance(interviewType: string, duration: number): Observable<{ hasBalance: boolean; cost: number; currentBalance: number }> {
    const params = new HttpParams()
      .set('type', interviewType)
      .set('duration', duration.toString());

    return this.http.get<{ hasBalance: boolean; cost: number; currentBalance: number }>
      (`${this.API_URL}/check-interview-balance`, { params });
  }

  // Deduct credits for interview
  deductInterviewCredits(interviewId: string, interviewType: string, duration: number): Observable<Transaction> {
    const request = {
      interviewId,
      interviewType,
      duration
    };

    return this.http.post<Transaction>(`${this.API_URL}/deduct-interview-credits`, request)
      .pipe(
        tap(() => {
          this.refreshUserBalance();
        })
      );
  }

  // Get available subscription plans
  getSubscriptionPlans(): Observable<SubscriptionPlan[]> {
    return this.http.get<SubscriptionPlan[]>(`${this.API_URL}/subscription-plans`)
      .pipe(
        tap(plans => {
          this.subscriptionPlansCache.next(plans);
        })
      );
  }

  // Get available credit packages
  getCreditPackages(): Observable<CreditPackage[]> {
    return this.http.get<CreditPackage[]>(`${this.API_URL}/credit-packages`)
      .pipe(
        tap(packages => {
          this.creditPackagesCache.next(packages);
        })
      );
  }

  // Get user's payment methods
  getPaymentMethods(): Observable<PaymentMethod[]> {
    return this.http.get<PaymentMethod[]>(`${this.API_URL}/payment-methods`);
  }

  // Add new payment method
  addPaymentMethod(paymentMethod: Partial<PaymentMethod>): Observable<PaymentMethod> {
    return this.http.post<PaymentMethod>(`${this.API_URL}/payment-methods`, paymentMethod);
  }

  // Update payment method
  updatePaymentMethod(methodId: string, updates: Partial<PaymentMethod>): Observable<PaymentMethod> {
    return this.http.put<PaymentMethod>(`${this.API_URL}/payment-methods/${methodId}`, updates);
  }

  // Delete payment method
  deletePaymentMethod(methodId: string): Observable<{ success: boolean }> {
    return this.http.delete<{ success: boolean }>(`${this.API_URL}/payment-methods/${methodId}`);
  }

  // Set default payment method
  setDefaultPaymentMethod(methodId: string): Observable<{ success: boolean }> {
    return this.http.post<{ success: boolean }>(`${this.API_URL}/payment-methods/${methodId}/set-default`, {});
  }

  // Get user subscription details
  getUserSubscription(): Observable<UserSubscription | null> {
    return this.http.get<UserSubscription | null>(`${this.API_URL}/subscription`);
  }

  // Get billing history
  getBillingHistory(): Observable<BillingHistory[]> {
    return this.http.get<BillingHistory[]>(`${this.API_URL}/billing-history`);
  }

  // Get balance notification settings
  getNotificationSettings(): Observable<BalanceNotificationSettings> {
    return this.http.get<BalanceNotificationSettings>(`${this.API_URL}/notification-settings`);
  }

  // Update balance notification settings
  updateNotificationSettings(settings: Partial<BalanceNotificationSettings>): Observable<BalanceNotificationSettings> {
    return this.http.put<BalanceNotificationSettings>(`${this.API_URL}/notification-settings`, settings);
  }

  // Refresh user balance from server
  refreshUserBalance(): void {
    this.getUserBalance().subscribe();
  }

  // Get interview cost configuration
  getInterviewCosts(): Observable<InterviewCost[]> {
    return this.http.get<InterviewCost[]>(`${this.API_URL}/interview-costs`);
  }

  // Admin methods for balance management
  
  // Get balance analytics (Admin only)
  getBalanceAnalytics(): Observable<BalanceAnalytics> {
    return this.http.get<BalanceAnalytics>(`${this.API_URL}/admin/analytics`);
  }

  // Get all user balances (Admin only)
  getAllUserBalances(page?: number, limit?: number): Observable<{ balances: UserBalance[]; total: number }> {
    let params = new HttpParams();
    if (page) params = params.set('page', page.toString());
    if (limit) params = params.set('limit', limit.toString());

    return this.http.get<{ balances: UserBalance[]; total: number }>(`${this.API_URL}/admin/user-balances`, { params });
  }

  // Adjust user balance (Admin only)
  adjustUserBalance(userId: string, amount: number, reason: string): Observable<Transaction> {
    const request = {
      userId,
      amount,
      reason,
      type: TransactionType.ADMIN_ADJUSTMENT
    };

    return this.http.post<Transaction>(`${this.API_URL}/admin/adjust-balance`, request);
  }

  // Get user balance by ID (Admin only)
  getUserBalanceById(userId: string): Observable<UserBalance> {
    return this.http.get<UserBalance>(`${this.API_URL}/admin/user-balance/${userId}`);
  }

  // Process refund (Admin only)
  processRefund(transactionId: string, amount: number, reason: string): Observable<Transaction> {
    const request = {
      transactionId,
      amount,
      reason
    };

    return this.http.post<Transaction>(`${this.API_URL}/admin/refund`, request);
  }

  // Utility methods

  // Format currency amount
  formatCurrency(amount: number): string {
    return new Intl.NumberFormat('en-US', {
      style: 'currency',
      currency: 'USD'
    }).format(amount);
  }

  // Format credits
  formatCredits(credits: number): string {
    return `${credits.toLocaleString()} credits`;
  }

  // Check if user has active subscription
  hasActiveSubscription(): boolean {
    const balance = this.userBalanceSubject.value;
    return balance?.subscriptionStatus === 'active';
  }

  // Get days until subscription expires
  getDaysUntilExpiry(): number {
    const balance = this.userBalanceSubject.value;
    if (!balance?.subscriptionExpiresAt) return 0;
    
    const now = new Date();
    const expiry = new Date(balance.subscriptionExpiresAt);
    const diffTime = expiry.getTime() - now.getTime();
    return Math.ceil(diffTime / (1000 * 60 * 60 * 24));
  }
}
