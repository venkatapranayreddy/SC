import { Injectable } from '@angular/core';
import { BaseService } from './base.service';
import { Observable } from 'rxjs';
import { HttpParams } from '@angular/common/http';
import { MemberAffiliationDto, ApprovalActionDto } from './member.service';

@Injectable({
  providedIn: 'root'
})
export class MemberAffiliationService extends BaseService {
  constructor() {
    super('/api/MemberAffiliation/');
  }

  getPendingApprovals(approverId: number, requestType?: 'MemberApproval' | 'AffiliationApproval'): Observable<MemberAffiliationDto[]> {
    let params = new HttpParams();
    if (requestType) {
      params = params.set('requestType', requestType);
    }
    return this.customGetRequest(approverId, 'pending-approvals', params);
  }

  getMemberApprovals(approverId: number): Observable<MemberAffiliationDto[]> {
    return this.customGetRequest(approverId, 'member-approvals', new HttpParams());
  }

  getAffiliationApprovals(managerId: number): Observable<MemberAffiliationDto[]> {
    return this.customGetRequest(managerId, 'affiliation-approvals', new HttpParams());
  }

  getMemberAffiliation(id: number): Observable<MemberAffiliationDto> {
    return this.getRequest(id, undefined);
  }

  approveOrReject(approverId: number, dto: ApprovalActionDto): Observable<any> {
    // Note: approverId is passed as query parameter, so we need to append it to the URL
    return this.postRequest(`approve?approverId=${approverId}`, JSON.stringify(dto));
  }
}

