import { inject, OnInit } from "@angular/core";
import { BsModalRef, BsModalService } from "ngx-bootstrap/modal";
import { ModalInformComponent } from "../core/modal-inform/modal-inform.component";
import { Observable } from "rxjs";
import { ModalConfirmComponent } from "../core/modal-confirm/modal-confirm.component";
export abstract class BasePage2 {
  key: string = ''
}
export abstract class BasePage{
  //bsModalRef?: BsModalRef
  //bsModalService: BsModalService = inject(BsModalService);

  //showInformModal(message: string, title: string): void {
  //  let s = {
  //    initialState: {
  //      modalMessage: {
  //        title: title,
  //        message: message
  //      }
  //    }
  //  }
  //  this.bsModalRef = this.bsModalService.show(ModalInformComponent, s)
  //}

  //showConfirmModal(message: string, title: string): Observable<boolean> {
  //  let s = {
  //    initialState: {
  //      modalMessage: {
  //        title: title,
  //        message: message
  //      }
  //    }
  //  }
  //  this.bsModalRef = this.bsModalService.show(ModalConfirmComponent, s)
  //  return this.bsModalRef.content.onChoiceSelected;
  //}
}
