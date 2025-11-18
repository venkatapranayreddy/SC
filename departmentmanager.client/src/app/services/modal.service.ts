import { inject, Injectable, signal } from "@angular/core";
import { BsModalRef, BsModalService, ModalOptions } from "ngx-bootstrap/modal";
import { Observable, take } from "rxjs";
import { ModalConfirmComponent } from "../core/modal-confirm/modal-confirm.component";
import { ModalInformComponent } from "../core/modal-inform/modal-inform.component";

@Injectable({ providedIn: "root" })
export class StkModalService {
  bsModalRef?: BsModalRef
  bsModalService: BsModalService = inject(BsModalService);
  private modals = signal<Array<BsModalRef>>([]);
  showInformModal(message: string, title = 'Alert', options?: ModalOptions): void {
    if (!options) {
      options = {
        initialState: {
          modalMessage: {
            title: title,
            message: message
          }
        },
      }
    }
    let s: ModalOptions = {
      initialState: {
        modalMessage: {
          title: title,
          message: message
        }
      },
    }
    this.bsModalRef = this.bsModalService.show(ModalInformComponent, s);
    this.storeModalRef();
  }

  showConfirmModal(message: string, title = 'Confirm'): Observable<boolean> {
    let s = {
      initialState: {
        modalMessage: {
          title: title,
          message: message
        }
      }
    }
    this.bsModalRef = this.bsModalService.show(ModalConfirmComponent, s)
    this.storeModalRef();
    //this.bsModalRef.onHidden?.pipe(
    //  take(1))
    //  .subscribe(x => console.log(x));
    return this.bsModalRef.content.onChoiceSelected;
  }
  private storeModalRef() {
    this.modals.update(modals => [...modals, this.bsModalRef!]);

    this.bsModalRef!.onHidden?.pipe(
      take(1))
      .subscribe(x =>
        this.modals.update(modals => [...modals.splice(modals.indexOf(x as BsModalRef), 1)]));
  }
}
