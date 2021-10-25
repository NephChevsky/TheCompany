import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { UserService } from '../_services/user.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit
{
	registerForm: FormGroup = new FormGroup({});
	loading = false;
	submitted = false;
	authentFailed = false;

	constructor(
		private formBuilder: FormBuilder,
		private router: Router,
		private userService: UserService)
	{ }

	ngOnInit(): void
	{
		this.registerForm = this.formBuilder.group({
			login: ['', [
				Validators.email,
				Validators.required]],
			password: ['', [Validators.required]
			]
		});
	}

	get f() { return this.registerForm.controls; }

	onSubmit()
	{
		this.submitted = true;

		if (this.registerForm.invalid)
		{
			return;
		}

		this.loading = true;
		this.userService.login(this.registerForm.value)
			.subscribe(
				() =>
				{
					this.router.navigate(['/']);
					return;
				},
				error =>
				{
					this.loading = false;
					if (error.status === 401)
						this.authentFailed = true;
				});
	}

}
