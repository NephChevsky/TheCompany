import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { Field } from '../../_models/field';

@Component({
	selector: 'app-layout',
	templateUrl: './layout.component.html',
	styleUrls: ['./layout.component.scss']
})
export class LayoutComponent implements OnInit
{
	@Input() public form: FormGroup = new FormGroup({});
	@Input() public fieldsData: Field[];
	@Input() public editMode: boolean;
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
