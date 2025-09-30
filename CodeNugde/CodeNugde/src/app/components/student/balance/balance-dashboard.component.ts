import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { BalanceService } from '../../../services/balance.service';
import { AuthService } from '../../../services/auth.service';
import {
  UserBalance,
  Transaction,
  SubscriptionPlan,
  CreditPackage,
  PaymentMethod,
  UserSubscription,
  BillingHistory,
  SubscriptionStatus
} from '../../../models/balance.model';

// Student balance dashboard component
@Component({
  selector: 'app-balance-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './balance-dashboard.component.html',
  styleUrl: './balance-dashboard.component.css'
})
export class BalanceDashboardComponent implements OnInit {
  // Balance data signals
  userBalance = signal<UserBalance | null>(null);
  transactions = signal<Transaction[]>([]);
  subscriptionPlans = signal<SubscriptionPlan[]>([]);
  creditPackages = signal<CreditPackage[]>([]);
  paymentMethods = signal<PaymentMethod[]>([]);
  userSubscription = signal<UserSubscription | null>(null);
  billingHistory = signal<BillingHistory[]>([]);
  
  // UI state signals
  isLoading = signal<boolean>(true);
  error = signal<string>('');
  selectedTab = signal<string>('overview');
  showAddPaymentModal = signal<boolean>(false);
  showPurchaseModal = signal<boolean>(false);
  selectedPackage = signal<CreditPackage | null>(null);
  selectedPlan = signal<SubscriptionPlan | null>(null);
  
  // Computed properties
  currentBalance = computed(() => this.userBalance()?.currentBalance || 0);
  subscriptionStatus = computed(() => this.userBalance()?.subscriptionStatus || SubscriptionStatus.NONE);
  hasActiveSubscription = computed(() => this.subscriptionStatus() === SubscriptionStatus.ACTIVE);
  daysUntilExpiry = computed(() => this.balanceService.getDaysUntilExpiry());

  // Math utility for templates
  Math = Math;
  
  // Tab options
  tabOptions = [
    { id: 'overview', label: 'Overview', icon: 'bi-speedometer2' },
    { id: 'transactions', label: 'Transactions', icon: 'bi-list-ul' },
    { id: 'subscription', label: 'Subscription', icon: 'bi-star' },
    { id: 'payment-methods', label: 'Payment Methods', icon: 'bi-credit-card' },
    { id: 'billing', label: 'Billing History', icon: 'bi-receipt' }
  ];

  constructor(
    private balanceService: BalanceService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadBalanceData();
  }

  // Load all balance-related data
  loadBalanceData(): void {
    this.isLoading.set(true);
    this.error.set('');

    // Load user balance
    this.balanceService.getUserBalance().subscribe({
      next: (balance) => {
        this.userBalance.set(balance);
        this.checkLoadingComplete();
      },
      error: (error) => {
        this.handleError('Failed to load balance information', error);
      }
    });

    // Load transaction history
    this.balanceService.getTransactionHistory(10).subscribe({
      next: (transactions) => {
        this.transactions.set(transactions);
        this.checkLoadingComplete();
      },
      error: (error) => {
        console.error('Failed to load transactions:', error);
      }
    });

    // Load subscription plans
    this.balanceService.getSubscriptionPlans().subscribe({
      next: (plans) => {
        this.subscriptionPlans.set(plans);
        this.checkLoadingComplete();
      },
      error: (error) => {
        console.error('Failed to load subscription plans:', error);
      }
    });

    // Load credit packages
    this.balanceService.getCreditPackages().subscribe({
      next: (packages) => {
        this.creditPackages.set(packages);
        this.checkLoadingComplete();
      },
      error: (error) => {
        console.error('Failed to load credit packages:', error);
      }
    });

    // Load payment methods
    this.balanceService.getPaymentMethods().subscribe({
      next: (methods) => {
        this.paymentMethods.set(methods);
        this.checkLoadingComplete();
      },
      error: (error) => {
        console.error('Failed to load payment methods:', error);
      }
    });

    // Load user subscription if exists
    this.balanceService.getUserSubscription().subscribe({
      next: (subscription) => {
        this.userSubscription.set(subscription);
        this.checkLoadingComplete();
      },
      error: (error) => {
        console.error('Failed to load subscription:', error);
      }
    });

    // Load billing history
    this.balanceService.getBillingHistory().subscribe({
      next: (history) => {
        this.billingHistory.set(history);
        this.checkLoadingComplete();
      },
      error: (error) => {
        console.error('Failed to load billing history:', error);
      }
    });
  }

  // Check if all data is loaded
  private checkLoadingComplete(): void {
    // Simple check - in real implementation, you might want more sophisticated loading state management
    if (this.userBalance()) {
      this.isLoading.set(false);
    }
  }

  // Handle errors
  private handleError(message: string, error: any): void {
    this.error.set(message);
    this.isLoading.set(false);
    console.error(message, error);
  }

  // Switch tabs
  selectTab(tabId: string): void {
    this.selectedTab.set(tabId);
  }

  // Get greeting message
  getGreeting(): string {
    const firstName = this.authService.currentUser()?.firstName || 'Student';
    return `Hello, ${firstName}!`;
  }

  // Get balance status class
  getBalanceStatusClass(): string {
    const balance = this.currentBalance();
    if (balance <= 0) return 'text-danger';
    if (balance < 50) return 'text-warning';
    return 'text-success';
  }

  // Get subscription status badge class
  getSubscriptionBadgeClass(): string {
    const status = this.subscriptionStatus();
    const classes: { [key: string]: string } = {
      'active': 'badge bg-success',
      'expired': 'badge bg-danger',
      'cancelled': 'badge bg-secondary',
      'pending': 'badge bg-warning',
      'none': 'badge bg-light text-dark'
    };
    return classes[status] || 'badge bg-secondary';
  }

  // Format transaction type
  formatTransactionType(type: string): string {
    return type.replace(/_/g, ' ').replace(/\b\w/g, l => l.toUpperCase());
  }

  // Get transaction icon
  getTransactionIcon(type: string): string {
    const icons: { [key: string]: string } = {
      'credit_purchase': 'bi-plus-circle text-success',
      'subscription_payment': 'bi-star text-primary',
      'interview_charge': 'bi-dash-circle text-warning',
      'refund': 'bi-arrow-clockwise text-info',
      'bonus_credit': 'bi-gift text-success',
      'admin_adjustment': 'bi-gear text-secondary'
    };
    return icons[type] || 'bi-circle text-muted';
  }

  // Show purchase credits modal
  showPurchaseCredits(creditPackage: CreditPackage): void {
    this.selectedPackage.set(creditPackage);
    this.showPurchaseModal.set(true);
  }

  // Show subscribe to plan modal
  showSubscribeToPlan(plan: SubscriptionPlan): void {
    this.selectedPlan.set(plan);
    this.showPurchaseModal.set(true);
  }

  // Purchase credits
  purchaseCredits(packageId: string, paymentMethodId: string): void {
    this.balanceService.purchaseCredits(packageId, paymentMethodId).subscribe({
      next: (response) => {
        if (response.success) {
          this.showPurchaseModal.set(false);
          this.loadBalanceData(); // Refresh data
          // Show success message
        } else {
          this.error.set('Payment failed. Please try again.');
        }
      },
      error: (error) => {
        this.error.set('Payment processing failed. Please try again.');
        console.error('Purchase error:', error);
      }
    });
  }

  // Subscribe to plan
  subscribeToPlan(planId: string, paymentMethodId: string): void {
    this.balanceService.subscribeToPlan(planId, paymentMethodId).subscribe({
      next: (response) => {
        if (response.success) {
          this.showPurchaseModal.set(false);
          this.loadBalanceData(); // Refresh data
          // Show success message
        } else {
          this.error.set('Subscription failed. Please try again.');
        }
      },
      error: (error) => {
        this.error.set('Subscription processing failed. Please try again.');
        console.error('Subscription error:', error);
      }
    });
  }

  // Cancel subscription
  cancelSubscription(): void {
    const subscription = this.userSubscription();
    if (!subscription) return;

    if (confirm('Are you sure you want to cancel your subscription?')) {
      this.balanceService.cancelSubscription(subscription.id).subscribe({
        next: (response) => {
          if (response.success) {
            this.loadBalanceData(); // Refresh data
            // Show success message
          }
        },
        error: (error) => {
          this.error.set('Failed to cancel subscription. Please try again.');
          console.error('Cancellation error:', error);
        }
      });
    }
  }

  // Add payment method
  addPaymentMethod(): void {
    this.showAddPaymentModal.set(true);
  }

  // Set default payment method
  setDefaultPaymentMethod(methodId: string): void {
    this.balanceService.setDefaultPaymentMethod(methodId).subscribe({
      next: () => {
        this.loadBalanceData(); // Refresh data
      },
      error: (error) => {
        this.error.set('Failed to update default payment method.');
        console.error('Payment method error:', error);
      }
    });
  }

  // Delete payment method
  deletePaymentMethod(methodId: string): void {
    if (confirm('Are you sure you want to delete this payment method?')) {
      this.balanceService.deletePaymentMethod(methodId).subscribe({
        next: () => {
          this.loadBalanceData(); // Refresh data
        },
        error: (error) => {
          this.error.set('Failed to delete payment method.');
          console.error('Payment method deletion error:', error);
        }
      });
    }
  }

  // Format date
  formatDate(date: Date | string): string {
    return new Date(date).toLocaleDateString();
  }

  // Format currency
  formatCurrency(amount: number): string {
    return this.balanceService.formatCurrency(amount);
  }

  // Format credits
  formatCredits(credits: number): string {
    return this.balanceService.formatCredits(credits);
  }

  // Close modals
  closeModals(): void {
    this.showPurchaseModal.set(false);
    this.showAddPaymentModal.set(false);
    this.selectedPackage.set(null);
    this.selectedPlan.set(null);
  }

  // Refresh data
  refreshData(): void {
    this.loadBalanceData();
  }
}
