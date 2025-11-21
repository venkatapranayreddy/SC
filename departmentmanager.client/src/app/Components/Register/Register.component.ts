import { Component, computed, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AbstractControl, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { StkModalService } from '../../services/modal.service';
import { MemberService } from '../../services/member.service';
import { BaseService } from '../../services/base.service';
import { HttpClient } from '@angular/common/http';
import { HttpParams } from '@angular/common/http';
import { catchError, forkJoin, of } from 'rxjs';

type RegistrationMode = 'google' | 'form' | null;

interface Approver {
  memberId: number;
  fullName: string;
  email: string;
  managerId?: number;
  managerName?: string;
}

interface Affiliation {
  affiliationId: number;
  name: string;
  cityId: number;
}

interface Role {
  roleId: number;
  name: string;
  affiliationId: number;
}

interface City {
  cityId: number;
  cityName: string;
}

@Component({
  selector: 'app-register',
  standalone: true,
  templateUrl: './Register.component.html',
  styleUrl: './Register.component.css',
  imports: [CommonModule, ReactiveFormsModule, FormsModule]
})
export class RegisterComponent implements OnInit {
  private modalService = inject(StkModalService);
  private fb = inject(FormBuilder);
  private memberService = inject(MemberService);
  private httpClient = inject(HttpClient);

  registrationMode = signal<RegistrationMode>(null);
  isSubmitting = signal(false);
  isLoading = signal(false);
  googleProfile = signal<{ fullName: string; email: string; googleId?: string } | null>(null);
  selectedCity = signal<number | null>(null);
  selectedAffiliation = signal<number | null>(null);

  cities = signal<City[]>([]);
  affiliations = signal<Affiliation[]>([]);
  roles = signal<Role[]>([]);
  approvers = signal<Approver[]>([]);

  registrationForm = this.fb.group({
    fullName: this.fb.control({ value: '', disabled: false }, Validators.required),
    email: this.fb.control(
      { value: '', disabled: false },
      [Validators.required, Validators.email, control => this.nonGmailValidator(control)]
    ),
    phoneNumber: this.fb.control('', [
      Validators.required,
      Validators.pattern(/^[0-9]{10}$/)
    ]),
    instagramId: this.fb.control(''),
    city: this.fb.control('', Validators.required),
    affiliation: this.fb.control('', Validators.required),
    role: this.fb.control('', Validators.required),
    approver: this.fb.control('', Validators.required),
    govtId: this.fb.control('', Validators.required),
    profilePicture: this.fb.control<File | null>(null, Validators.required),
    address: this.fb.control('', [Validators.required, Validators.minLength(10)]),
    digitalSignature: this.fb.control<File | null>(null, Validators.required),
    termsAccepted: this.fb.control(false, Validators.requiredTrue)
  });

  affiliationOptions = computed(() => {
    const cityId = this.selectedCity();
    if (!cityId) {
      return [] as Affiliation[];
    }
    return this.affiliations().filter(aff => aff.cityId === cityId);
  });

  roleOptions = computed(() => {
    const affiliationId = this.selectedAffiliation();
    if (!affiliationId) {
      return [] as Role[];
    }
    return this.roles().filter(role => role.affiliationId === affiliationId);
  });

  approverOptions = computed(() => {
    return this.approvers();
  });

  constructor() {
    this.registrationForm.get('city')?.valueChanges.subscribe(cityId => {
      this.selectedCity.set(cityId ? parseInt(cityId) : null);
      this.registrationForm.get('affiliation')?.setValue('');
      this.registrationForm.get('role')?.setValue('');
      this.registrationForm.get('approver')?.setValue('');
      this.selectedAffiliation.set(null);
      this.loadAffiliationsForCity(cityId ? parseInt(cityId) : null);
    });

    this.registrationForm.get('affiliation')?.valueChanges.subscribe(affiliationId => {
      this.selectedAffiliation.set(affiliationId ? parseInt(affiliationId) : null);
      this.registrationForm.get('role')?.setValue('');
      this.registrationForm.get('approver')?.setValue('');
      this.loadRolesForAffiliation(affiliationId ? parseInt(affiliationId) : null);
      this.loadApproversForAffiliation(affiliationId ? parseInt(affiliationId) : null);
    });
  }

  ngOnInit(): void {
    this.loadCities();
  }

  private loadCities(): void {
    this.isLoading.set(true);
    this.httpClient.get<City[]>('/api/City/')
      .pipe(
        catchError(error => {
          this.modalService.showInformModal('Failed to load cities. Please refresh the page.', 'Error');
          return of([]);
        })
      )
      .subscribe(cities => {
        this.cities.set(cities);
        this.isLoading.set(false);
      });
  }

  private loadAffiliationsForCity(cityId: number | null): void {
    if (!cityId) {
      this.affiliations.set([]);
      return;
    }

    this.httpClient.get<Affiliation[]>(`/api/Affilation/?cityId=${cityId}`)
      .pipe(
        catchError(error => {
          console.error('Error loading affiliations:', error);
          return of([]);
        })
      )
      .subscribe(affiliations => {
        this.affiliations.set(affiliations);
      });
  }

  private loadRolesForAffiliation(affiliationId: number | null): void {
    if (!affiliationId) {
      this.roles.set([]);
      return;
    }

    this.httpClient.get<Role[]>(`/api/Roles/?affiliationId=${affiliationId}`)
      .pipe(
        catchError(error => {
          console.error('Error loading roles:', error);
          return of([]);
        })
      )
      .subscribe(roles => {
        this.roles.set(roles);
      });
  }

  private loadApproversForAffiliation(affiliationId: number | null): void {
    if (!affiliationId) {
      this.approvers.set([]);
      return;
    }

    this.httpClient.get<Approver[]>(`/api/Member/approvers/${affiliationId}`)
      .pipe(
        catchError(error => {
          console.error('Error loading approvers:', error);
          return of([]);
        })
      )
      .subscribe(approvers => {
        this.approvers.set(approvers);
      });
  }

  get fullNameControl() {
    return this.registrationForm.get('fullName');
  }

  get emailControl() {
    return this.registrationForm.get('email');
  }

  onSelectMode(mode: Exclude<RegistrationMode, null>): void {
    this.registrationMode.set(mode);

    if (mode === 'google') {
      this.startGoogleSignUp();
    } else {
      this.googleProfile.set(null);
      this.fullNameControl?.enable();
      this.emailControl?.enable();
      this.registrationForm.patchValue({
        fullName: '',
        email: ''
      });
      this.emailControl?.updateValueAndValidity();
    }
  }

  resetMode(): void {
    this.registrationMode.set(null);
    this.googleProfile.set(null);
    this.registrationForm.reset({
      termsAccepted: false
    });
    this.selectedCity.set(null);
    this.selectedAffiliation.set(null);
  }

  onFileChange(event: Event, controlName: 'profilePicture' | 'digitalSignature'): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;
    this.registrationForm.get(controlName)?.setValue(file);
    this.registrationForm.get(controlName)?.markAsTouched();
  }

  async onSubmit(): Promise<void> {
    if (!this.registrationMode()) {
      this.modalService.showInformModal('Please select how you would like to sign up (Google or Fill out).', 'Select Sign Up Mode');
      return;
    }

    this.registrationForm.markAllAsTouched();

    if (this.registrationForm.invalid) {
      this.modalService.showInformModal('Please complete all required fields before submitting.', 'Missing Information');
      return;
    }

    this.isSubmitting.set(true);

    try {
      // Upload files first
      const profilePictureFile = this.registrationForm.get('profilePicture')?.value as File;
      const digitalSignatureFile = this.registrationForm.get('digitalSignature')?.value as File;

      const [profilePictureUrl, digitalSignatureUrl] = await Promise.all([
        this.memberService.uploadFile(profilePictureFile).toPromise().then(r => r?.url || ''),
        this.memberService.uploadFile(digitalSignatureFile).toPromise().then(r => r?.url || '')
      ]);

      const formValue = this.registrationForm.getRawValue();
      const registrationDto: any = {
        fullName: formValue.fullName,
        email: formValue.email,
        phoneNumber: formValue.phoneNumber,
        instagramId: formValue.instagramId || undefined,
        address: formValue.address,
        cityId: formValue.city ? parseInt(formValue.city) : undefined,
        affiliationId: formValue.affiliation ? parseInt(formValue.affiliation) : undefined,
        roleId: formValue.role ? parseInt(formValue.role) : undefined,
        approverId: formValue.approver ? parseInt(formValue.approver) : undefined,
        govtId: formValue.govtId,
        profilePictureUrl: profilePictureUrl,
        digitalSignatureUrl: digitalSignatureUrl,
        acceptTermsAndConditions: formValue.termsAccepted
      };

      if (this.registrationMode() === 'google' && this.googleProfile()) {
        registrationDto.googleId = this.googleProfile()?.googleId || 'google-oauth-id';
        registrationDto.googleEmail = this.googleProfile()?.email;
      }

      this.memberService.registerMember(registrationDto).subscribe({
        next: (response) => {
          this.isSubmitting.set(false);
          this.modalService.showInformModal(
            'Registration submitted successfully. Your approver will be notified for affiliation approval. After your approver and their manager approve, your affiliation account will be activated. You can then sign in to view your affiliations and dashboards.',
            'Registration Submitted'
          );
          this.resetMode();
        },
        error: (error) => {
          this.isSubmitting.set(false);
          const errorMessage = error?.error?.message || 'Registration failed. Please try again.';
          this.modalService.showInformModal(errorMessage, 'Registration Error');
        }
      });
    } catch (error) {
      this.isSubmitting.set(false);
      this.modalService.showInformModal('Failed to upload files. Please try again.', 'Upload Error');
    }
  }

  private startGoogleSignUp(): void {
    // TODO: Implement actual Google OAuth 2.0 flow
    // For now, show a placeholder
    this.modalService.showInformModal(
      'Google Sign-Up will redirect to OAuth 2.0 provider in production. Prefilling your Google profile information for now.',
      'Google Sign-Up (Preview)'
    );

    // In production, this would be handled by Google OAuth
    // For now, use a sample profile
    const sampleProfile = {
      fullName: 'Street Cause Member',
      email: 'member@streetcause.org',
      googleId: 'google-oauth-id-placeholder'
    };

    this.googleProfile.set(sampleProfile);
    this.fullNameControl?.setValue(sampleProfile.fullName);
    this.emailControl?.setValue(sampleProfile.email);

    this.fullNameControl?.disable();
    this.emailControl?.disable();
  }

  private nonGmailValidator(control: AbstractControl) {
    const value = control.value?.toLowerCase();
    if (!value || this.registrationMode() !== 'form') {
      return null;
    }

    return value.endsWith('@gmail.com') ? { gmailAddressNotAllowed: true } : null;
  }
}
