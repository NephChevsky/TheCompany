import { Component, OnInit } from '@angular/core';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { UserService } from '../_services/user.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-signup',
	templateUrl: './signup.component.html',
	styleUrls: ['./signup.component.scss']
})
export class SignUpComponent implements OnInit
{
	registerForm: FormGroup = new FormGroup({});
	loading = false;
	submitted = false;

	constructor(
		private formBuilder: FormBuilder,
		private router: Router,
		private userService: UserService) { }

	ngOnInit()
	{
		this.registerForm = this.formBuilder.group({
			email: ['', [
				Validators.email,
				Validators.required]],
			password: ['', [
				Validators.required,
				Validators.minLength(6)]],
			confirmPassword: ['', [
				Validators.required,
				Validators.minLength(6)]]
		}, { validator: this.checkPasswords() });
	}

	get f() { return this.registerForm.controls; }

	checkPasswords()
	{
		return (group: FormGroup) =>
		{
			let passwordInput = group.controls['password'],
				passwordConfirmationInput = group.controls['confirmPassword'];
			if (passwordInput.value !== passwordConfirmationInput.value)
			{
				return passwordConfirmationInput.setErrors({ notEquals: true })
			}
			else
			{
				return passwordConfirmationInput.setErrors(null);
			}
		}
	}
  
	onSubmit()
	{
		this.submitted = true;

		if (this.registerForm.invalid)
		{
			return;
		}

		this.loading = true;
		var user = {
			email: this.registerForm.get("email").value,
			password: this.registerForm.get("password").value
		}
		this.userService.register(user)
			.subscribe(
				data =>
				{
					this.router.navigate(['/']);
					return;
				},
				error =>
				{
					this.loading = false;
         			if (error.status == 409)
						this.registerForm.controls['email'].setErrors({ taken: true });
				});
	}
}
