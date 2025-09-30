// Balance and payment related models for the mock interview platform

// User balance information
export interface UserBalance {
  userId: string;
  currentBalance: number; // in credits
  totalSpent: number;
  totalEarned: number;
  lastUpdated: Date;
  subscriptionStatus: SubscriptionStatus;
  subscriptionPlan?: SubscriptionPlan;
  subscriptionExpiresAt?: Date;
}

// Subscription status enum
export enum SubscriptionStatus {
  NONE = 'none',
  ACTIVE = 'active',
  EXPIRED = 'expired',
  CANCELLED = 'cancelled',
  PENDING = 'pending'
}

// Subscription plans available
export interface SubscriptionPlan {
  id: string;
  name: string;
  description: string;
  price: number; // monthly price
  credits: number; // credits included per month
  features: string[];
  duration: SubscriptionDuration;
  isPopular: boolean;
  isActive: boolean;
  createdAt: Date;
}

// Subscription duration options
export enum SubscriptionDuration {
  MONTHLY = 'monthly',
  QUARTERLY = 'quarterly',
  YEARLY = 'yearly'
}

// Transaction record for all balance changes
export interface Transaction {
  id: string;
  userId: string;
  type: TransactionType;
  amount: number; // positive for credits, negative for debits
  description: string;
  status: TransactionStatus;
  paymentMethod?: PaymentMethod;
  relatedEntityId?: string; // interview ID, subscription ID, etc.
  relatedEntityType?: string;
  createdAt: Date;
  processedAt?: Date;
  failureReason?: string;
}

// Transaction types
export enum TransactionType {
  CREDIT_PURCHASE = 'credit_purchase',
  SUBSCRIPTION_PAYMENT = 'subscription_payment',
  INTERVIEW_CHARGE = 'interview_charge',
  REFUND = 'refund',
  BONUS_CREDIT = 'bonus_credit',
  ADMIN_ADJUSTMENT = 'admin_adjustment'
}

// Transaction status
export enum TransactionStatus {
  PENDING = 'pending',
  COMPLETED = 'completed',
  FAILED = 'failed',
  CANCELLED = 'cancelled',
  REFUNDED = 'refunded'
}

// Payment method information
export interface PaymentMethod {
  id: string;
  userId: string;
  type: PaymentMethodType;
  provider: PaymentProvider;
  last4Digits?: string;
  expiryMonth?: number;
  expiryYear?: number;
  cardBrand?: string;
  isDefault: boolean;
  isActive: boolean;
  billingAddress?: BillingAddress;
  createdAt: Date;
}

// Payment method types
export enum PaymentMethodType {
  CREDIT_CARD = 'credit_card',
  DEBIT_CARD = 'debit_card',
  PAYPAL = 'paypal',
  BANK_TRANSFER = 'bank_transfer',
  DIGITAL_WALLET = 'digital_wallet'
}

// Payment providers
export enum PaymentProvider {
  STRIPE = 'stripe',
  PAYPAL = 'paypal',
  RAZORPAY = 'razorpay',
  SQUARE = 'square'
}

// Billing address
export interface BillingAddress {
  firstName: string;
  lastName: string;
  addressLine1: string;
  addressLine2?: string;
  city: string;
  state: string;
  postalCode: string;
  country: string;
}

// Credit package for purchasing credits
export interface CreditPackage {
  id: string;
  name: string;
  credits: number;
  price: number;
  bonusCredits: number;
  description: string;
  isPopular: boolean;
  isActive: boolean;
  validUntil?: Date;
}

// Billing history for subscriptions
export interface BillingHistory {
  id: string;
  userId: string;
  subscriptionId: string;
  amount: number;
  billingPeriodStart: Date;
  billingPeriodEnd: Date;
  status: BillingStatus;
  invoiceUrl?: string;
  paymentMethod: PaymentMethod;
  createdAt: Date;
  paidAt?: Date;
}

// Billing status
export enum BillingStatus {
  PENDING = 'pending',
  PAID = 'paid',
  FAILED = 'failed',
  OVERDUE = 'overdue',
  CANCELLED = 'cancelled'
}

// User subscription details
export interface UserSubscription {
  id: string;
  userId: string;
  planId: string;
  plan: SubscriptionPlan;
  status: SubscriptionStatus;
  startDate: Date;
  endDate: Date;
  autoRenew: boolean;
  paymentMethod: PaymentMethod;
  creditsUsed: number;
  creditsRemaining: number;
  billingHistory: BillingHistory[];
  createdAt: Date;
  cancelledAt?: Date;
  cancellationReason?: string;
}

// Balance analytics for admin dashboard
export interface BalanceAnalytics {
  totalRevenue: number;
  monthlyRevenue: number;
  totalActiveSubscriptions: number;
  totalCreditsInCirculation: number;
  averageSpendingPerUser: number;
  topSpendingUsers: UserSpendingSummary[];
  revenueByPlan: { [planId: string]: number };
  transactionVolume: TransactionVolumeSummary;
}

// User spending summary
export interface UserSpendingSummary {
  userId: string;
  userName: string;
  totalSpent: number;
  subscriptionPlan?: string;
  lastActivity: Date;
}

// Transaction volume summary
export interface TransactionVolumeSummary {
  daily: number;
  weekly: number;
  monthly: number;
  totalTransactions: number;
  averageTransactionValue: number;
}

// Payment processing request
export interface PaymentRequest {
  amount: number;
  currency: string;
  paymentMethodId: string;
  description: string;
  metadata?: { [key: string]: any };
}

// Payment processing response
export interface PaymentResponse {
  success: boolean;
  transactionId: string;
  amount: number;
  status: TransactionStatus;
  message?: string;
  errorCode?: string;
}

// Credit pricing configuration
export interface CreditPricing {
  basePrice: number; // price per credit
  bulkDiscounts: BulkDiscount[];
  subscriptionDiscounts: SubscriptionDiscount[];
}

// Bulk discount tiers
export interface BulkDiscount {
  minCredits: number;
  discountPercentage: number;
  description: string;
}

// Subscription-based discounts
export interface SubscriptionDiscount {
  planId: string;
  discountPercentage: number;
  description: string;
}

// Interview cost configuration
export interface InterviewCost {
  type: string; // interview type
  baseCost: number; // in credits
  durationMultiplier: number;
  difficultyMultiplier: { [difficulty: string]: number };
}

// Balance notification settings
export interface BalanceNotificationSettings {
  userId: string;
  lowBalanceThreshold: number;
  emailNotifications: boolean;
  pushNotifications: boolean;
  subscriptionReminders: boolean;
  paymentFailureAlerts: boolean;
}
