import { Component, ElementRef, EventEmitter, Input, OnInit, Output, Self, ViewChild } from '@angular/core';
import { ControlValueAccessor, NgControl } from '@angular/forms';
@Component({
  selector: 'app-text-input',
  templateUrl: './text-input.component.html',
  styleUrls: ['./text-input.component.scss']
})
export class TextInputComponent implements OnInit, ControlValueAccessor {
[x: string]: any;
  @ViewChild('input', {static: true}) input: ElementRef;
  @Input() type = 'text';
  @Input() typeDate = 'date';
  @Input() label: string;
  @Input() pattern = '';
  @Input() textcase = 'upper';
  @Input() readonly=false;
  @Input() disabled: boolean = false;
 // @Input() appendtext = null;
  @Output() blurEvent: EventEmitter<any> = new EventEmitter();
  @Output() focusEvent: EventEmitter<any> = new EventEmitter();

  constructor( @Self() public controlDir: NgControl) { 
    this.controlDir.valueAccessor = this;
  }
  ngOnInit(): void {debugger;
    if (this.type === 'date' && !this.typeDate) 
      this.typeDate = 'date';
    const control = this.controlDir.control;

    // const validators = control?.validator ? [control.validator]: [];
    // const asyncValidators = control?.asyncValidator ? [control?.asyncValidator] : [];
    this.input.nativeElement.setAttribute('data-textcase', this.textcase);
    // control?.setValidators(validators);
    // control?.setAsyncValidators(asyncValidators);
    // control?.updateValueAndValidity();   
  }
  changecase(t:any) {
     //// console.log(t);
  }

  onBlur(event: FocusEvent) {
    this.blurEvent.emit(event);  // Emit blur event
    this.onTouched(); 
  }
 //onFocus(event: FocusEvent) {
  onFocus(event:any){
  this.focusEvent.emit(event);  // Emit blur event
  this.onTouched(); 
 }
  onTouched() {
    //  // console.log('onTouched');
  }

  writeValue(obj: any): void {
    this.input.nativeElement.value = obj || '';
  }

  registerOnChange(fn: any): void {
    //  // console.log('registerOnChange');
    this.onChange = fn;
  }

  registerOnTouched(fn: any): void {
    //  // console.log('registerOnTouched');
    this.onTouched = fn;
  }
  onChange(event:any) {
    // Can pass the updated value back to the form control
    if (this.controlDir.control) {
      this.controlDir.control.setValue(event.target.value);
    }
  }
  setDisabledState?(isDisabled: boolean): void {
    this.disabled = isDisabled;
  }

}
