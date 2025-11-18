import { Component, OnInit } from '@angular/core';
import { ModalMessage } from '../../models/modal-message';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-modal-inform',
  templateUrl: './modal-inform.component.html',
  styleUrl: './modal-inform.component.css',
  standalone: false
})
export class ModalInformComponent implements OnInit {

  modalMessage: ModalMessage = { message: '' };
  css: string = '';
  constructor(public bsModalRef: BsModalRef) {

  }

  ngOnInit(): void {
  }

}
