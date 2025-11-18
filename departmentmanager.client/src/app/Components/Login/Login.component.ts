import { Component, inject, signal } from '@angular/core';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';
import { StkModalService } from '../../services/modal.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './Login.component.html',
  styleUrl: './Login.component.css'
})
export class LoginComponent {
  private authService = inject(AuthService);
  private router = inject(Router);
  private modalService = inject(StkModalService);

  isLoading = signal(false);
  
  loginForm = new FormGroup({
    email: new FormControl('', [
      Validators.required,
      Validators.email
    ]),
    password: new FormControl('', [
      Validators.required,
      Validators.minLength(6)
    ])
  });

  get email() {
    return this.loginForm.get('email');
  }

  get password() {
    return this.loginForm.get('password');
  }

  onSubmit(): void {
    if (this.loginForm.valid) {
      this.isLoading.set(true);
      const email = this.loginForm.value.email!;
      const password = this.loginForm.value.password!;

      // TODO: Implement actual login logic with AuthService
      this.performLogin(email, password);
    } else {
      this.loginForm.markAllAsTouched();
      this.modalService.showInformModal('Please fill in all required fields correctly.', 'Validation Error');
    }
  }

  private performLogin(email: string, password: string): void {
    // Placeholder for actual authentication
    // This should call your backend API through AuthService
    console.log('Login attempt:', { email, password });
    
    // Simulate API call
    setTimeout(() => {
      this.isLoading.set(false);
      // TODO: Handle successful login - store token, redirect, etc.
      // this.router.navigate(['/dashboard']);
      this.modalService.showInformModal('Login functionality will be connected to backend.', 'Info');
    }, 1000);
  }

  onGoogleSignIn(): void {
    this.isLoading.set(true);
    
    // TODO: Implement Google OAuth flow
    // This should redirect to your backend OAuth endpoint or use Google Identity Services
    console.log('Google Sign-In initiated');
    
    // Placeholder for Google OAuth
    // Option 1: Redirect to backend OAuth endpoint
    // window.location.href = '/api/auth/google';
    
    // Option 2: Use Google Identity Services (requires setup)
    // this.initializeGoogleSignIn();
    
    setTimeout(() => {
      this.isLoading.set(false);
      this.modalService.showInformModal('Google Sign-In will be connected to OAuth provider.', 'Info');
    }, 1000);
  }

  // Helper method for Google Identity Services (requires additional setup)
  // private initializeGoogleSignIn(): void {
  //   // This requires Google Identity Services library
  //   // See: https://developers.google.com/identity/gsi/web
  // }
}

