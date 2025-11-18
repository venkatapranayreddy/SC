import { Component, output } from '@angular/core';
import { ModalMessage } from '../../models/modal-message';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-modal-confirm',
  standalone: false,
  templateUrl: './modal-confirm.component.html',
  styleUrl: './modal-confirm.component.css'
})
export class ModalConfirmComponent {

  modalMessage: ModalMessage = { message: '' };
  onChoiceSelected = output<boolean>();

  constructor(public bsModalRef: BsModalRef) {
  }

  ngOnInit(): void {
  }

  confirm(): void {
    this.onChoiceSelected.emit(true);
    this.bsModalRef.hide();
  }

  deny(): void {
    this.onChoiceSelected.emit(false);
    this.bsModalRef.hide();
  }
}
