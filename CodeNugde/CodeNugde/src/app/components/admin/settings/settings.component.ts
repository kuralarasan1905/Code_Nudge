import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule, ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, ReactiveFormsModule],
  templateUrl: './settings.component.html',
  styleUrl: './settings.component.css'
})
export class SettingsComponent implements OnInit {
  // Settings data signals
  isLoading = signal<boolean>(false);
  isSaving = signal<boolean>(false);
  error = signal<string>('');
  successMessage = signal<string>('');
  
  // Active tab signal
  activeTab = signal<string>('general');
  
  // Forms
  generalForm: FormGroup;
  emailForm: FormGroup;
  securityForm: FormGroup;
  interviewForm: FormGroup;
  
  // Tab options
  tabs = [
    { id: 'general', label: 'General', icon: 'bi-gear' },
    { id: 'email', label: 'Email', icon: 'bi-envelope' },
    { id: 'security', label: 'Security', icon: 'bi-shield-check' },
    { id: 'interview', label: 'Interview', icon: 'bi-clipboard-data' },
    { id: 'backup', label: 'Backup', icon: 'bi-cloud-download' }
  ];

  constructor(private fb: FormBuilder) {
    // Initialize forms
    this.generalForm = this.fb.group({
      platformName: ['CodeNugde', [Validators.required]],
      platformDescription: ['Interview preparation platform', [Validators.required]],
      supportEmail: ['support@codenugde.com', [Validators.required, Validators.email]],
      timezone: ['UTC', [Validators.required]],
      language: ['en', [Validators.required]],
      maintenanceMode: [false],
      allowRegistration: [true],
      requireEmailVerification: [true]
    });

    this.emailForm = this.fb.group({
      smtpHost: ['smtp.gmail.com', [Validators.required]],
      smtpPort: [587, [Validators.required, Validators.min(1), Validators.max(65535)]],
      smtpUsername: ['', [Validators.required]],
      smtpPassword: ['', [Validators.required]],
      smtpEncryption: ['tls', [Validators.required]],
      fromEmail: ['noreply@codenugde.com', [Validators.required, Validators.email]],
      fromName: ['CodeNugde Platform', [Validators.required]]
    });

    this.securityForm = this.fb.group({
      sessionTimeout: [30, [Validators.required, Validators.min(5), Validators.max(480)]],
      maxLoginAttempts: [5, [Validators.required, Validators.min(3), Validators.max(10)]],
      passwordMinLength: [8, [Validators.required, Validators.min(6), Validators.max(20)]],
      requireSpecialChars: [true],
      requireNumbers: [true],
      requireUppercase: [true],
      enableTwoFactor: [false],
      ipWhitelist: [''],
      corsOrigins: ['http://localhost:4200']
    });

    this.interviewForm = this.fb.group({
      defaultDuration: [60, [Validators.required, Validators.min(15), Validators.max(180)]],
      maxConcurrentInterviews: [100, [Validators.required, Validators.min(1), Validators.max(1000)]],
      autoSaveInterval: [30, [Validators.required, Validators.min(10), Validators.max(300)]],
      codeExecutionTimeout: [10, [Validators.required, Validators.min(5), Validators.max(60)]],
      allowCodeUpload: [true],
      enableVideoRecording: [false],
      enableScreenSharing: [true],
      proctoring: [false]
    });
  }

  ngOnInit(): void {
    this.loadSettings();
  }

  // Load current settings
  loadSettings(): void {
    this.isLoading.set(true);
    this.error.set('');

    // Mock implementation - replace with actual service call
    setTimeout(() => {
      this.isLoading.set(false);
    }, 1000);
  }

  // Set active tab
  setActiveTab(tabId: string): void {
    this.activeTab.set(tabId);
  }

  // Save general settings
  saveGeneralSettings(): void {
    if (this.generalForm.valid) {
      this.isSaving.set(true);
      this.error.set('');
      this.successMessage.set('');

      // Mock implementation
      setTimeout(() => {
        this.isSaving.set(false);
        this.successMessage.set('General settings saved successfully!');
        setTimeout(() => this.successMessage.set(''), 3000);
      }, 1000);
    }
  }

  // Save email settings
  saveEmailSettings(): void {
    if (this.emailForm.valid) {
      this.isSaving.set(true);
      this.error.set('');
      this.successMessage.set('');

      // Mock implementation
      setTimeout(() => {
        this.isSaving.set(false);
        this.successMessage.set('Email settings saved successfully!');
        setTimeout(() => this.successMessage.set(''), 3000);
      }, 1000);
    }
  }

  // Test email configuration
  testEmailConfig(): void {
    if (this.emailForm.valid) {
      this.isSaving.set(true);
      this.error.set('');
      this.successMessage.set('');

      // Mock implementation
      setTimeout(() => {
        this.isSaving.set(false);
        this.successMessage.set('Test email sent successfully!');
        setTimeout(() => this.successMessage.set(''), 3000);
      }, 2000);
    }
  }

  // Save security settings
  saveSecuritySettings(): void {
    if (this.securityForm.valid) {
      this.isSaving.set(true);
      this.error.set('');
      this.successMessage.set('');

      // Mock implementation
      setTimeout(() => {
        this.isSaving.set(false);
        this.successMessage.set('Security settings saved successfully!');
        setTimeout(() => this.successMessage.set(''), 3000);
      }, 1000);
    }
  }

  // Save interview settings
  saveInterviewSettings(): void {
    if (this.interviewForm.valid) {
      this.isSaving.set(true);
      this.error.set('');
      this.successMessage.set('');

      // Mock implementation
      setTimeout(() => {
        this.isSaving.set(false);
        this.successMessage.set('Interview settings saved successfully!');
        setTimeout(() => this.successMessage.set(''), 3000);
      }, 1000);
    }
  }

  // Create backup
  createBackup(): void {
    this.isSaving.set(true);
    this.error.set('');
    this.successMessage.set('');

    // Mock implementation
    setTimeout(() => {
      this.isSaving.set(false);
      this.successMessage.set('Backup created successfully!');
      setTimeout(() => this.successMessage.set(''), 3000);
      
      // Simulate download
      if (typeof window !== 'undefined') {
        const blob = new Blob(['Mock backup data'], { type: 'application/json' });
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = `codenugde-backup-${new Date().toISOString().split('T')[0]}.json`;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        window.URL.revokeObjectURL(url);
      }
    }, 2000);
  }

  // Restore from backup
  restoreBackup(event: any): void {
    const file = event.target.files[0];
    if (file) {
      this.isSaving.set(true);
      this.error.set('');
      this.successMessage.set('');

      // Mock implementation
      setTimeout(() => {
        this.isSaving.set(false);
        this.successMessage.set('Backup restored successfully!');
        setTimeout(() => this.successMessage.set(''), 3000);
      }, 3000);
    }
  }

  // Clear cache
  clearCache(): void {
    this.isSaving.set(true);
    this.error.set('');
    this.successMessage.set('');

    // Mock implementation
    setTimeout(() => {
      this.isSaving.set(false);
      this.successMessage.set('Cache cleared successfully!');
      setTimeout(() => this.successMessage.set(''), 3000);
    }, 1000);
  }

  // Reset to defaults
  resetToDefaults(): void {
    if (confirm('Are you sure you want to reset all settings to default values? This action cannot be undone.')) {
      this.isSaving.set(true);
      this.error.set('');
      this.successMessage.set('');

      // Mock implementation
      setTimeout(() => {
        this.isSaving.set(false);
        this.successMessage.set('Settings reset to defaults successfully!');
        setTimeout(() => this.successMessage.set(''), 3000);
        this.loadSettings(); // Reload settings
      }, 1000);
    }
  }

  // Get form for active tab
  getActiveForm(): FormGroup {
    switch (this.activeTab()) {
      case 'general': return this.generalForm;
      case 'email': return this.emailForm;
      case 'security': return this.securityForm;
      case 'interview': return this.interviewForm;
      default: return this.generalForm;
    }
  }

  // Check if active form is valid
  isActiveFormValid(): boolean {
    return this.getActiveForm().valid;
  }
}
