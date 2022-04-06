import {Directive, ElementRef, EventEmitter, HostBinding, HostListener, Input, Output} from '@angular/core';
import {DecimalPipe} from "@angular/common";

@Directive({
  selector: '[turkishLira]'
})
export class TurkishLiraDirective {
  private navigationKeys = [
    'Backspace',
    'Delete',
    'Tab',
    'Escape',
    'Enter',
    'Home',
    'End',
    'ArrowLeft',
    'ArrowRight',
    'Clear',
    'Copy',
    'Paste'
  ];
  format = '0.2-2';

  @HostBinding('value')
  stringValue: any;


  @Output() appNumberInputChange: any = new EventEmitter();

  constructor(private decimalPipe: DecimalPipe, private el: ElementRef) {
  }

  ngOnInit() {
    this.stringValue = this.decimalPipe.transform(this.el.nativeElement.value, this.format);
  }
  @HostListener('keydown', ['$event'])
  onKeyDown(e: KeyboardEvent) {
    if (
      this.navigationKeys.indexOf(e.key) > -1 ||
      (e.key === '.') || (e.key === ',')
    ) {
      // let it happen, don't do anything
      return;
    }
    // Ensure that it is a number and stop the keypress
    if (
      (e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) &&
      (e.keyCode < 96 || e.keyCode > 105)
    ) {
      e.preventDefault();
    }
  }
  @HostListener('blur', ['$event.target.value'])
  @HostListener('keyup.enter', ['$event.target.value'])
  formatANumber(value: any) {
    const numberValue = parseFloat(value.replace('.', '').replace(',', '.'));
    this.stringValue = this.decimalPipe.transform(numberValue, this.format, 'tr-TR');
    this.appNumberInputChange.next(numberValue);
  }
}
