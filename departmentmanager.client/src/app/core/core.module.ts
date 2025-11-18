import { NgModule } from "@angular/core";
import { BsModalService, ModalModule } from "ngx-bootstrap/modal";
import { TooltipModule } from "ngx-bootstrap/tooltip";
import { StkFormControlDirective } from "./form-controls/input.directive";
import { StkModalService } from "../services/modal.service";


@NgModule({
  declarations: [
    StkFormControlDirective
  ],
  imports: [],
  exports: [
    TooltipModule,
    StkFormControlDirective
  ],
  providers: [BsModalService, StkModalService]
  
})
export class CoreModule { }
