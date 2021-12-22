import { DataSource } from '@angular/cdk/collections';
import { Component, EventEmitter, Input, OnInit, Output, ViewEncapsulation } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { DomSanitizer } from '@angular/platform-browser';
import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
import { Field } from '../../_models/field';
import { FieldService } from '../../_services/field.service';

@Component({
  selector: 'app-field',
  templateUrl: './field.component.html',
	styleUrls: ['./field.component.scss'],
	encapsulation: ViewEncapsulation.None
})
export class FieldComponent implements OnInit {

	@Input()
	public field: Field
	@Input()
	public form: FormGroup;
	@Input()
	public index: number = null;
	@Input()
	public size: string = "400";
	@Input()
	public rows: string = "3";
	@Input()
	public cols: string = "51";
	@Input()
	public editMode: boolean;
	public image: any;
	@Output() focusEvent = new EventEmitter<EventTarget>();
	public filteredValues: Observable<string[]>;
	private updateBindingsMethod = function(event : any) { };

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

		if (this.field.autoCompletable)
		{
			this.filteredValues = this.form.get(this.field.name).valueChanges.pipe(
				startWith(''),
				map(value => this.filter(value)),
			);

			var obj = {
				dataSource: this.field.dataSource,
				name: this.field.name
			}
			this.fieldService.getPossibleValues(obj)
				.subscribe((data: any) =>
				{
					this.field.possibleValues = data;
				}, error =>
				{
					// TODO: handle errors
				});
		}

		if (this.field.bindings)
		{
			this.updateBindingsMethod = function (event: any)
			{
				var obj = {
					dataSource: this.field.bindings.dataSource,
					name: this.field.bindings.name,
					value: this.form.get(this.field.bindings.name).value
				}
				this.fieldService.getBindingValues(obj)
					.subscribe((data: any) =>
					{
						if (data.length != 0)
						{
							for (let key in this.field.bindings.childs)
							{
								this.form.get(this.field.bindings.childs[key]).setValue(data.find((x: any) => x.name == key).value);
							}
						}
					}, error =>
					{
						// TODO: handle errors
					});
			}
		}
	}

	filter(value: string): string[]
	{
		const filterValue = value.toLowerCase();
		return this.field.possibleValues.filter(option => option.toLowerCase().includes(filterValue));
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

	updateBindings(event: any)
	{
		this.updateBindingsMethod(event);
	}
}
