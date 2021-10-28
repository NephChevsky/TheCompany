import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AdditionalFieldService } from 'src/app/_services/additionalField.service';

@Component({
  selector: 'app-add',
  templateUrl: './add.component.html',
  styleUrls: ['./add.component.scss']
})
export class AddComponent implements OnInit
{
	public dataSource: string = "";
	public addFieldForm : FormGroup = new FormGroup({});
	constructor(private formBuilder: FormBuilder,
				private route: ActivatedRoute,
				private router: Router,
				private additionalFieldService: AdditionalFieldService) { }

	ngOnInit(): void {
		this.dataSource = this.route.snapshot.paramMap.get('dataSource');
		this.addFieldForm = this.formBuilder.group({
			dataSource: [this.dataSource, [Validators.required]],
			name: ['', [Validators.required]]
		});
	}

	onSubmit()
	{
		if (this.addFieldForm.invalid)
		{
			return;
		}
		this.additionalFieldService.createCustomer(this.addFieldForm.value)
			.subscribe(
				data =>
				{
					this.router.navigate(['Home']);
					return;
				},
				error =>
				{
					/* TODO: set errors on field */
				});
	}

	onCancel()
	{
		this.router.navigate(['Home']);
	}
}
