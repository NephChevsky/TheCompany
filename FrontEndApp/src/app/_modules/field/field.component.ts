import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { DomSanitizer } from '@angular/platform-browser';
import { Field } from '../../_models/field';
import { FieldService } from '../../_services/field.service';

@Component({
  selector: 'app-field',
  templateUrl: './field.component.html',
  styleUrls: ['./field.component.scss']
})
export class FieldComponent implements OnInit {

	@Input()
	public field: Field
	@Input()
	public form: FormGroup;
	@Input()
	public index: number = null;
	@Input()
	public size: string = "50";
	@Input()
	public rows: string = "3";
	@Input()
	public cols: string = "51";
	@Input()
	public editMode: boolean;
	public image: any;
	@Output() focusEvent = new EventEmitter<EventTarget>();

	constructor(private fieldService: FieldService, private sanitizer: DomSanitizer) { }

	ngOnInit(): void
	{
		if (this.field.type == "FileField")
		{
			this.fieldService.getFile(this.field.value)
				.subscribe((data: any) =>
				{
					var binary = '';
					var bytes = new Uint8Array(data);
					var len = bytes.byteLength;
					for (var i = 0; i < len; i++)
					{
						binary += String.fromCharCode(bytes[i]);
					}
					this.image = this.sanitizer.bypassSecurityTrustResourceUrl("data:image/png;base64," + binary);
				}, error =>
				{
					// TODO: handle errors
				});
		}
	}

	onFocus(target: EventTarget)
	{
		this.focusEvent.emit(target);
	}

	onFileChange(event: any)
	{
		if (event.target.files.length > 0)
		{
			const file = event.target.files[0];
			var fileObj = this.form.get(event.currentTarget.id);
			if (fileObj)
				fileObj.setValue(file);
		}
		else
		{
			var fileObj = this.form.get(event.currentTarget.id);
			if (fileObj)
				fileObj.setValue(null);
		}
	}
}
