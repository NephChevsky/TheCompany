import { Component, EventEmitter, Input, OnInit, Output, SimpleChanges } from '@angular/core';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'app-text-field',
  templateUrl: './text-field.component.html',
  styleUrls: ['./text-field.component.scss']
})
export class TextFieldComponent implements OnInit {

	@Input()
	public form: FormGroup;
	@Input()
	public key: string = "";
	@Input()
	public value: string = "";
	@Input()
	public size: string = "50";
	@Input()
	public readonlyValue: boolean;
	@Output() focusEvent = new EventEmitter<EventTarget>();

	constructor() { }

	ngOnInit(): void {
	}

	onFocus(target: EventTarget)
	{
		this.focusEvent.emit(target);
	}
}
