import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'app-field',
  templateUrl: './field.component.html',
  styleUrls: ['./field.component.scss']
})
export class FieldComponent implements OnInit {

	@Input()
	public type: string
	@Input()
	public form: FormGroup;
	@Input()
	public key: string = "";
	@Input()
	public value: string = "";
	@Input()
	public size: string = "50";
	@Input()
	public rows: string = "3";
	@Input()
	public cols: string = "51";
	@Input()
	public readonlyValue: boolean;
	@Output() focusEvent = new EventEmitter<EventTarget>();

	constructor() { }

	ngOnInit(): void
	{
	}

	onFocus(target: EventTarget)
	{
		this.focusEvent.emit(target);
	}
}
