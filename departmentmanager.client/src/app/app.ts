import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NavbarComponent } from './core/navbar/navbar.component';
import { CoreModule } from './core/core.module';
import { FormControl, FormGroup, FormsModule, NgForm, Validators } from '@angular/forms';
import { BasePage } from './features/base.component';
import { TestService } from './services/test.service';
import { StkModalService } from './services/modal.service';
import { LoginComponent } from './Components/Login/Login.component';
import { RegisterComponent } from './Components/Register/Register.component';
import { MemberApprovalComponent } from './Components/MemberApproval/MemberApproval.component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, NavbarComponent, CoreModule, FormsModule, LoginComponent, RegisterComponent, MemberApprovalComponent],
  templateUrl: './app.html',
  styleUrl: './app.css',
  providers: [TestService]
})
export class App extends BasePage {

  protected readonly title = signal('TITLE');
  testService = inject(TestService);
  modalService = inject(StkModalService);
  value = 'TEST'

  constructor() { super() }

  showConfirm() {

    this.modalService.showConfirmModal('SERVICE', 'SERVICE').subscribe(x => {
      if (x) console.log('Y')
      else console.log('N')
    }, );

  }
  showInform() {
    this.modalService.showInformModal('message', 'title');
  }
  test() {
    console.log(this.value)
    this.testService.cachedGet().subscribe(x => console.log('Sub Result', x));
  }
}
