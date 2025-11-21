import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { StkModalService } from '../../services/modal.service';
import { MemberAffiliationService } from '../../services/member-affiliation.service';
import { MemberAffiliationDto, ApprovalActionDto } from '../../services/member.service';

@Component({
  selector: 'app-member-approval',
  standalone: true,
  templateUrl: './MemberApproval.component.html',
  styleUrl: './MemberApproval.component.css',
  imports: [CommonModule, FormsModule]
})
export class MemberApprovalComponent implements OnInit {
  private modalService = inject(StkModalService);
  private memberAffiliationService = inject(MemberAffiliationService);

  activeTab = signal<'member' | 'affiliation'>('member');
  isLoading = signal(false);
  memberApprovals = signal<MemberAffiliationDto[]>([]);
  affiliationApprovals = signal<MemberAffiliationDto[]>([]);
  selectedApproval = signal<MemberAffiliationDto | null>(null);
  showRejectionReason = signal(false);
  rejectionReason = signal('');
  approverId = signal<number | null>(null); // This should come from auth service

  ngOnInit(): void {
    // TODO: Get approverId from auth service
    // For now, using a placeholder - in production, get from logged-in user
    const approverId = 1; // Replace with actual approver ID from auth
    this.approverId.set(approverId);
    this.loadApprovals();
  }

  loadApprovals(): void {
    const approverId = this.approverId();
    if (!approverId) {
      this.modalService.showInformModal('Approver ID not found. Please log in.', 'Authentication Required');
      return;
    }

    this.isLoading.set(true);

    // Load member approvals
    this.memberAffiliationService.getMemberApprovals(approverId).subscribe({
      next: (approvals) => {
        this.memberApprovals.set(approvals);
        this.isLoading.set(false);
      },
      error: (error) => {
        this.isLoading.set(false);
        this.modalService.showInformModal('Failed to load member approvals.', 'Error');
      }
    });

    // Load affiliation approvals
    this.memberAffiliationService.getAffiliationApprovals(approverId).subscribe({
      next: (approvals) => {
        this.affiliationApprovals.set(approvals);
      },
      error: (error) => {
        console.error('Error loading affiliation approvals:', error);
      }
    });
  }

  switchTab(tab: 'member' | 'affiliation'): void {
    this.activeTab.set(tab);
    this.selectedApproval.set(null);
    this.showRejectionReason.set(false);
    this.rejectionReason.set('');
  }

  showDetails(approval: MemberAffiliationDto): void {
    this.selectedApproval.set(approval);
  }

  closeDetails(): void {
    this.selectedApproval.set(null);
    this.showRejectionReason.set(false);
    this.rejectionReason.set('');
  }

  approve(approval: MemberAffiliationDto): void {
    const approverId = this.approverId();
    if (!approverId) {
      this.modalService.showInformModal('Approver ID not found.', 'Error');
      return;
    }

    const dto: ApprovalActionDto = {
      memberAffiliationId: approval.memberAffiliationId,
      isApproved: true
    };

    this.memberAffiliationService.approveOrReject(approverId, dto).subscribe({
      next: () => {
        this.modalService.showInformModal('Approval submitted successfully.', 'Success');
        this.loadApprovals();
        this.closeDetails();
      },
      error: (error) => {
        const errorMessage = error?.error?.message || 'Failed to submit approval.';
        this.modalService.showInformModal(errorMessage, 'Error');
      }
    });
  }

  reject(approval: MemberAffiliationDto): void {
    this.showRejectionReason.set(true);
  }

  submitRejection(approval: MemberAffiliationDto): void {
    if (!this.rejectionReason().trim()) {
      this.modalService.showInformModal('Please provide a reason for rejection.', 'Validation Error');
      return;
    }

    const approverId = this.approverId();
    if (!approverId) {
      this.modalService.showInformModal('Approver ID not found.', 'Error');
      return;
    }

    const dto: ApprovalActionDto = {
      memberAffiliationId: approval.memberAffiliationId,
      isApproved: false,
      rejectionReason: this.rejectionReason()
    };

    this.memberAffiliationService.approveOrReject(approverId, dto).subscribe({
      next: () => {
        this.modalService.showInformModal('Rejection submitted successfully.', 'Success');
        this.loadApprovals();
        this.closeDetails();
      },
      error: (error) => {
        const errorMessage = error?.error?.message || 'Failed to submit rejection.';
        this.modalService.showInformModal(errorMessage, 'Error');
      }
    });
  }

  cancelRejection(): void {
    this.showRejectionReason.set(false);
    this.rejectionReason.set('');
  }
}

