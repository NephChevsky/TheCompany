import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { ClearAllService } from 'src/app/_services/clearall.service';

@Component({
	selector: 'app-clear-all',
	templateUrl: './clear-all.component.html',
	styleUrls: ['./clear-all.component.scss']
})
export class ClearAllComponent implements OnInit {

	clearAllForm: FormGroup = new FormGroup({});

	constructor(private formBuilder: FormBuilder,
				private clearAllService: ClearAllService,
				private router: Router) { }

	ngOnInit(): void {
		this.clearAllForm = this.formBuilder.group({
			invoices: [true, []],
			customers: [true, []],
			extractionSettings: [true, []]
		});
	}

	onSubmit() {
		debugger;
		this.clearAllService.clearAll(this.clearAllForm.value).subscribe(result => {
			this.router.navigate(["Home"]);
		}, error => {
			// TODO
		});
	}

	onCancel() {
		this.router.navigate(["Home"]);
	}
}
