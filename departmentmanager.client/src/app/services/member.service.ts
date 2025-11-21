import { Injectable, inject } from '@angular/core';
import { BaseService } from './base.service';
import { Observable } from 'rxjs';
import { HttpParams } from '@angular/common/http';

export interface RegisterMemberDto {
  fullName: string;
  email: string;
  phoneNumber: string;
  googleId?: string;
  googleEmail?: string;
  instagramId?: string;
  address?: string;
  cityId: number;
  affiliationId: number;
  roleId: number;
  approverId: number;
  govtId: string;
  profilePictureUrl: string;
  digitalSignatureUrl: string;
  acceptTermsAndConditions: boolean;
}

export interface MemberDetailDto {
  memberId: number;
  fullName: string;
  email: string;
  phoneNumber: string;
  instagramId?: string;
  address?: string;
  googleId?: string;
  googleEmail?: string;
  status: string;
  affiliations: MemberAffiliationDto[];
}

export interface MemberAffiliationDto {
  memberAffiliationId: number;
  memberId: number;
  memberName: string;
  memberEmail: string;
  memberPhoneNumber: string;
  cityId: number;
  cityName: string;
  affiliationId: number;
  affiliationName: string;
  roleId: number;
  roleName: string;
  approverId?: number;
  approverName?: string;
  managerName?: string;
  govtId: string;
  profilePictureUrl: string;
  digitalSignatureUrl: string;
  approvalStatus: string;
  rejectionReason?: string;
  createdAt: string;
}

export interface ApprovalActionDto {
  memberAffiliationId: number;
  isApproved: boolean;
  rejectionReason?: string;
}

@Injectable({
  providedIn: 'root'
})
export class MemberService extends BaseService {
  constructor() {
    super('/api/Member/');
  }

  registerMember(dto: RegisterMemberDto): Observable<MemberDetailDto> {
    return this.postRequest('register', JSON.stringify(dto));
  }

  getMember(id: number): Observable<MemberDetailDto> {
    return this.getRequest(id, undefined);
  }

  getMemberByEmail(email: string): Observable<MemberDetailDto> {
    return this.customGetRequest(null, `email/${encodeURIComponent(email)}`, new HttpParams());
  }

  getMemberAffiliations(memberId: number): Observable<MemberAffiliationDto[]> {
    return this.customGetRequest(memberId, 'affiliations', new HttpParams());
  }

  uploadFile(file: File): Observable<{ url: string }> {
    // This would typically upload to a file storage service
    // For now, return a placeholder URL
    return new Observable(observer => {
      // In a real implementation, upload to Azure Blob Storage, AWS S3, etc.
      const reader = new FileReader();
      reader.onload = () => {
        // Convert to base64 for now (not ideal for production)
        const base64 = reader.result as string;
        observer.next({ url: base64 });
        observer.complete();
      };
      reader.onerror = error => observer.error(error);
      reader.readAsDataURL(file);
    });
  }
}

