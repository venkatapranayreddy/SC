import { Directive, ElementRef, HostListener, inject, input, Renderer2, signal, Signal } from '@angular/core';
@Directive({
  selector: 'input[stkFormControl]',
  standalone: false
})
export class StkFormControlDirective {
  private classList =
  [
    'form-control'
  ];

  private el = inject(ElementRef);
  private renderer = inject(Renderer2);

  addClasses = input(true);
  excludeClass = input<Array<string>>([]);

  constructor() {
    this.addClassList();
  }

  private addClassList(): void {
    this.classList.forEach(className => {
      if (!this.hasClass(className) && !this.excludeClass().includes(className))
        this.renderer.addClass(this.el.nativeElement, className);
    })
  }

  private hasClass(className: string): boolean {
    return this.el.nativeElement.classList.contains(className);
  }
}
