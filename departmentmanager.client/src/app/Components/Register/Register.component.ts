import { Component, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AbstractControl, FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { FormsModule } from '@angular/forms';
import { StkModalService } from '../../services/modal.service';

type RegistrationMode = 'google' | 'form' | null;

interface Approver {
  id: string;
  name: string;
  manager: string;
}

interface Affiliation {
  id: string;
  name: string;
  approvers: Approver[];
}

interface City {
  id: string;
  name: string;
  affiliations: Affiliation[];
}

@Component({
  selector: 'app-register',
  standalone: true,
  templateUrl: './Register.component.html',
  styleUrl: './Register.component.css',
  imports: [CommonModule, ReactiveFormsModule, FormsModule]
})
export class RegisterComponent {
  private modalService = inject(StkModalService);
  private fb = inject(FormBuilder);

  registrationMode = signal<RegistrationMode>(null);
  isSubmitting = signal(false);
  googleProfile = signal<{ fullName: string; email: string } | null>(null);
  selectedCity = signal<string | null>(null);
  selectedAffiliation = signal<string | null>(null);

  roles = [
    { id: 'member', label: 'Member' },
    { id: 'lead', label: 'Team Lead' },
    { id: 'coordinator', label: 'Coordinator' },
    { id: 'finance', label: 'Finance' },
    { id: 'admin', label: 'Admin' }
  ];

  cities: City[] = [
    {
      id: 'hyd',
      name: 'Hyderabad',
      affiliations: [
        {
          id: 'hyd-edu',
          name: 'Street Cause Hyderabad - Education',
          approvers: [
            { id: 'anna-rao', name: 'Ananya Rao', manager: 'Nikhil Varma' },
            { id: 'rahul-k', name: 'Rahul Kumar', manager: 'Nikhil Varma' }
          ]
        },
        {
          id: 'hyd-health',
          name: 'Street Cause Hyderabad - Health',
          approvers: [
            { id: 'sana-p', name: 'Sana Patel', manager: 'Rohit Salian' }
          ]
        }
      ]
    },
    {
      id: 'blr',
      name: 'Bengaluru',
      affiliations: [
        {
          id: 'blr-ops',
          name: 'Street Cause Bengaluru - Operations',
          approvers: [
            { id: 'meera-d', name: 'Meera Dsouza', manager: 'Anirudh Bhat' }
          ]
        },
        {
          id: 'blr-tech',
          name: 'Street Cause Bengaluru - Technology',
          approvers: [
            { id: 'akash-k', name: 'Akash Kulkarni', manager: 'Anirudh Bhat' },
            { id: 'nisha-fern', name: 'Nisha Fernandes', manager: 'Uma Devi' }
          ]
        }
      ]
    },
    {
      id: 'viz',
      name: 'Vizag',
      affiliations: [
        {
          id: 'viz-community',
          name: 'Street Cause Vizag - Community',
          approvers: [
            { id: 'jahnavi-s', name: 'Jahnavi Shenoy', manager: 'Abhinav Rao' }
          ]
        }
      ]
    }
  ];

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
    return this.cities.find(city => city.id === cityId)?.affiliations ?? [];
  });

  approverOptions = computed(() => {
    const affiliationId = this.selectedAffiliation();
    if (!affiliationId) {
      return [] as Approver[];
    }
    return this.affiliationOptions().find(aff => aff.id === affiliationId)?.approvers ?? [];
  });

  constructor() {
    this.registrationForm.get('city')?.valueChanges.subscribe(cityId => {
      this.selectedCity.set(cityId || null);
      this.registrationForm.get('affiliation')?.setValue('');
      this.registrationForm.get('approver')?.setValue('');
      this.selectedAffiliation.set(null);
    });

    this.registrationForm.get('affiliation')?.valueChanges.subscribe(affiliationId => {
      this.selectedAffiliation.set(affiliationId || null);
      this.registrationForm.get('approver')?.setValue('');
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

  onSubmit(): void {
    if (!this.registrationMode()) {
      this.modalService.showInformModal('Please select how you would like to sign up (Google or Fill out).', 'Select Sign Up Mode');
      return;
    }

    this.registrationForm.markAllAsTouched();

    if (this.registrationForm.invalid) {
      this.modalService.showInformModal('Please complete all required fields before submitting.', 'Missing Information');
      return;
    }

    const payload = {
      mode: this.registrationMode(),
      googleProfile: this.googleProfile(),
      ...this.registrationForm.getRawValue()
    };

    console.log('Registration payload', payload);
    this.isSubmitting.set(true);

    setTimeout(() => {
      this.isSubmitting.set(false);
      this.modalService.showInformModal(
        'Registration submitted successfully. Your approver will be notified for affiliation approval. After your approver and their manager approve, your affiliation account will be activated. You can then sign in to view your affiliations and dashboards.',
        'Registration Submitted'
      );
      this.resetMode();
    }, 1200);
  }

  private startGoogleSignUp(): void {
    this.modalService.showInformModal(
      'Google Sign-Up will redirect to OAuth 2.0 provider in production. Prefilling your Google profile information for now.',
      'Google Sign-Up (Preview)'
    );

    const sampleProfile = {
      fullName: 'Street Cause Member',
      email: 'member@streetcause.org'
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
