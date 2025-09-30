import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService, RegisterRequest } from '../../../services/auth.service';
import { UserRole } from '../../../models/user.model';

// Registration component for new user signup
@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  // Reactive form for registration
  registerForm: FormGroup;
  
  // Component state signals
  isLoading = signal<boolean>(false);
  errorMessage = signal<string>('');
  successMessage = signal<string>('');
  showPassword = signal<boolean>(false);
  showConfirmPassword = signal<boolean>(false);
  currentStep = signal<number>(1);
  
  // Available user roles for registration
  userRoles = [
    { value: UserRole.STUDENT, label: 'Student', description: 'Practice coding and take mock interviews' },
    { value: UserRole.ADMIN, label: 'Admin', description: 'Manage platform and create content' }
  ];

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    // Initialize registration form with validation
    this.registerForm = this.fb.group({
      // Step 1: Basic Information
      firstName: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(50), Validators.pattern(/^[a-zA-Z\s]+$/)]],
      lastName: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(50), Validators.pattern(/^[a-zA-Z\s]+$/)]],
      email: ['', [Validators.required, Validators.email, Validators.maxLength(255)]],
      role: [UserRole.STUDENT, [Validators.required]],
      phoneNumber: ['', [Validators.pattern(/^\+?[1-9]\d{1,14}$/)]],

      // Step 2: Account Security
      password: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(100), this.passwordValidator]],
      confirmPassword: ['', [Validators.required]],

      // Step 3: Additional Information (role-specific)
      college: ['', [Validators.maxLength(200)]],
      branch: ['', [Validators.maxLength(100)]],
      graduationYear: ['', [Validators.min(2020), Validators.max(2030)]],
      registerId: ['', [Validators.maxLength(50)]], // For students - will be required conditionally
      employeeId: ['', [Validators.maxLength(50)]], // For admins - will be required conditionally

      // Terms and conditions
      acceptTerms: [false, [Validators.requiredTrue]]
    }, { validators: [this.passwordMatchValidator, this.roleSpecificValidator] });

    // Watch for role changes to update validation
    this.registerForm.get('role')?.valueChanges.subscribe(() => {
      this.registerForm.updateValueAndValidity();
    });
  }

  // Custom password validator - matches backend requirements
  passwordValidator(control: any) {
    const value = control.value;
    if (!value) return null;

    const hasNumber = /[0-9]/.test(value);
    const hasUpper = /[A-Z]/.test(value);
    const hasLower = /[a-z]/.test(value);

    // Backend only requires lowercase, uppercase, and digit (no special character required)
    const valid = hasNumber && hasUpper && hasLower;
    return valid ? null : { passwordStrength: true };
  }

  // Password match validator
  passwordMatchValidator(form: FormGroup) {
    const password = form.get('password');
    const confirmPassword = form.get('confirmPassword');

    if (password && confirmPassword && password.value !== confirmPassword.value) {
      confirmPassword.setErrors({ passwordMismatch: true });
      return { passwordMismatch: true };
    }

    return null;
  }

  // Role-specific validator for RegisterId and EmployeeId
  roleSpecificValidator(form: FormGroup) {
    const role = form.get('role')?.value;
    const registerId = form.get('registerId');
    const employeeId = form.get('employeeId');

    if (role === UserRole.STUDENT) {
      // RegisterId is required for students
      if (!registerId?.value || registerId.value.trim() === '') {
        registerId?.setErrors({ required: true });
        return { registerIdRequired: true };
      } else {
        // Clear any existing errors if value is provided
        if (registerId.errors?.['required']) {
          delete registerId.errors['required'];
          if (Object.keys(registerId.errors).length === 0) {
            registerId.setErrors(null);
          }
        }
      }
    } else if (role === UserRole.ADMIN) {
      // EmployeeId is required for admins
      if (!employeeId?.value || employeeId.value.trim() === '') {
        employeeId?.setErrors({ required: true });
        return { employeeIdRequired: true };
      } else {
        // Clear any existing errors if value is provided
        if (employeeId.errors?.['required']) {
          delete employeeId.errors['required'];
          if (Object.keys(employeeId.errors).length === 0) {
            employeeId.setErrors(null);
          }
        }
      }
    }

    return null;
  }

  // Handle form submission
  onSubmit(): void {
    if (this.registerForm.valid && !this.isLoading()) {
      this.isLoading.set(true);
      this.errorMessage.set('');
      this.successMessage.set('');

      const registerData: RegisterRequest = {
        firstName: this.registerForm.value.firstName,
        lastName: this.registerForm.value.lastName,
        email: this.registerForm.value.email,
        password: this.registerForm.value.password,
        confirmPassword: this.registerForm.value.confirmPassword,
        role: this.registerForm.value.role,
        college: this.registerForm.value.college,
        branch: this.registerForm.value.branch,
        graduationYear: this.registerForm.value.graduationYear,
        phoneNumber: this.registerForm.value.phoneNumber,
        registerId: this.registerForm.value.registerId,
        employeeId: this.registerForm.value.employeeId
      };

      // Attempt user registration
      this.authService.register(registerData).subscribe({
        next: (response) => {
          this.isLoading.set(false);
          if (response.success) {
            this.successMessage.set('Registration successful! Redirecting to dashboard...');
            // Navigation handled by auth service
          } else {
            this.errorMessage.set(response.message || 'Registration failed');
          }
        },
        error: (error) => {
          this.isLoading.set(false);
          this.errorMessage.set('An error occurred during registration. Please try again.');
          console.error('Registration error:', error);
        }
      });
    } else {
      this.markFormGroupTouched();
    }
  }

  // Navigate to next step
  nextStep(): void {
    if (this.currentStep() < 3) {
      // Validate current step before proceeding
      if (this.validateCurrentStep()) {
        this.currentStep.set(this.currentStep() + 1);
      }
    }
  }

  // Navigate to previous step
  previousStep(): void {
    if (this.currentStep() > 1) {
      this.currentStep.set(this.currentStep() - 1);
    }
  }

  // Validate current step fields
  validateCurrentStep(): boolean {
    const step = this.currentStep();
    let fieldsToValidate: string[] = [];

    switch (step) {
      case 1:
        fieldsToValidate = ['firstName', 'lastName', 'email', 'role'];
        break;
      case 2:
        fieldsToValidate = ['password', 'confirmPassword'];
        break;
      case 3:
        if (this.registerForm.value.role === UserRole.STUDENT) {
          fieldsToValidate = ['university', 'graduationYear', 'registerId'];
        } else if (this.registerForm.value.role === UserRole.ADMIN) {
          fieldsToValidate = ['employeeId'];
        }
        break;
    }

    let isValid = true;
    fieldsToValidate.forEach(field => {
      const control = this.registerForm.get(field);
      if (control) {
        control.markAsTouched();
        if (control.invalid) {
          isValid = false;
        }
      }
    });

    return isValid;
  }

  // Toggle password visibility
  togglePasswordVisibility(field: 'password' | 'confirmPassword'): void {
    if (field === 'password') {
      this.showPassword.set(!this.showPassword());
    } else {
      this.showConfirmPassword.set(!this.showConfirmPassword());
    }
  }

  // Check if form field has error
  hasFieldError(fieldName: string): boolean {
    const field = this.registerForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  // Get field error message
  getFieldError(fieldName: string): string {
    const field = this.registerForm.get(fieldName);
    if (field && field.errors) {
      if (field.errors['required']) {
        return `${this.getFieldDisplayName(fieldName)} is required`;
      }
      if (field.errors['email']) {
        return 'Please enter a valid email address';
      }
      if (field.errors['minlength']) {
        const requiredLength = field.errors['minlength'].requiredLength;
        return `${this.getFieldDisplayName(fieldName)} must be at least ${requiredLength} characters`;
      }
      if (field.errors['passwordStrength']) {
        return 'Password must contain uppercase, lowercase, number, and special character';
      }
      if (field.errors['passwordMismatch']) {
        return 'Passwords do not match';
      }
      if (field.errors['min']) {
        return `Year must be at least ${field.errors['min'].min}`;
      }
      if (field.errors['max']) {
        return `Year must be at most ${field.errors['max'].max}`;
      }
      if (field.errors['requiredTrue']) {
        return 'You must accept the terms and conditions';
      }
    }
    return '';
  }

  // Get display name for form fields
  private getFieldDisplayName(fieldName: string): string {
    const displayNames: { [key: string]: string } = {
      firstName: 'First Name',
      lastName: 'Last Name',
      email: 'Email',
      password: 'Password',
      confirmPassword: 'Confirm Password',
      phoneNumber: 'Phone Number',
      college: 'College/University',
      branch: 'Branch/Department',
      graduationYear: 'Graduation Year',
      registerId: 'Register ID',
      employeeId: 'Employee ID'
    };
    return displayNames[fieldName] || fieldName;
  }

  // Mark all form fields as touched
  private markFormGroupTouched(): void {
    Object.keys(this.registerForm.controls).forEach(key => {
      const control = this.registerForm.get(key);
      control?.markAsTouched();
    });
  }

  // Navigate to login page
  goToLogin(): void {
    this.router.navigate(['/auth/login']);
  }

  // Check if student role is selected
  isStudentRole(): boolean {
    return this.registerForm.value.role === UserRole.STUDENT;
  }

  // Check if admin role is selected
  isAdminRole(): boolean {
    return this.registerForm.value.role === UserRole.ADMIN;
  }

  // Update validators based on selected role
  onRoleChange(): void {
    const role = this.registerForm.value.role;
    const registerIdControl = this.registerForm.get('registerId');
    const employeeIdControl = this.registerForm.get('employeeId');
    const collegeControl = this.registerForm.get('college');
    const branchControl = this.registerForm.get('branch');
    const graduationYearControl = this.registerForm.get('graduationYear');

    // Clear existing validators
    registerIdControl?.clearValidators();
    employeeIdControl?.clearValidators();
    collegeControl?.clearValidators();
    branchControl?.clearValidators();
    graduationYearControl?.clearValidators();

    if (role === UserRole.STUDENT) {
      // Set validators for student fields
      registerIdControl?.setValidators([Validators.required, Validators.minLength(3)]);
      collegeControl?.setValidators([Validators.required, Validators.minLength(2)]);
      branchControl?.setValidators([Validators.required, Validators.minLength(2)]);
      graduationYearControl?.setValidators([Validators.required, Validators.min(2020), Validators.max(2030)]);
    } else if (role === UserRole.ADMIN) {
      // Set validators for admin fields
      employeeIdControl?.setValidators([Validators.required, Validators.minLength(3)]);
    }

    // Update validity
    registerIdControl?.updateValueAndValidity();
    employeeIdControl?.updateValueAndValidity();
    collegeControl?.updateValueAndValidity();
    branchControl?.updateValueAndValidity();
    graduationYearControl?.updateValueAndValidity();
  }

  // Get password strength
  getPasswordStrength(): number {
    const password = this.registerForm.value.password;
    if (!password) return 0;

    let strength = 0;
    if (password.length >= 8) strength += 25;
    if (/[a-z]/.test(password)) strength += 25;
    if (/[A-Z]/.test(password)) strength += 25;
    if (/[0-9]/.test(password)) strength += 12.5;
    if (/[#?!@$%^&*-]/.test(password)) strength += 12.5;

    return Math.min(strength, 100);
  }

  // Get password strength label
  getPasswordStrengthLabel(): string {
    const strength = this.getPasswordStrength();
    if (strength < 25) return 'Weak';
    if (strength < 50) return 'Fair';
    if (strength < 75) return 'Good';
    return 'Strong';
  }

  // Get password strength color
  getPasswordStrengthColor(): string {
    const strength = this.getPasswordStrength();
    if (strength < 25) return 'danger';
    if (strength < 50) return 'warning';
    if (strength < 75) return 'info';
    return 'success';
  }

  // Get password strength CSS class
  getPasswordStrengthClass(): string {
    const strength = this.getPasswordStrength();
    if (strength < 25) return 'weak';
    if (strength < 50) return 'fair';
    if (strength < 75) return 'good';
    return 'strong';
  }

  // Get password strength text
  getPasswordStrengthText(): string {
    return this.getPasswordStrengthLabel();
  }



  // Check if step 1 is valid
  isStep1Valid(): boolean {
    const controls = ['firstName', 'lastName', 'email', 'role'];
    return controls.every(control => {
      const formControl = this.registerForm.get(control);
      return formControl && formControl.valid;
    });
  }

  // Check if step 2 is valid
  isStep2Valid(): boolean {
    const controls = ['password', 'confirmPassword'];
    return controls.every(control => {
      const formControl = this.registerForm.get(control);
      return formControl && formControl.valid;
    });
  }
}