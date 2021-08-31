import React, { useState } from "react";
import {
	StyleSheet,
	Text,
	View,
	Image,
	TextInput,
	Platform,
	TouchableOpacity
} from "react-native";
import { User } from "../models/User";
import {ApiUrl} from "@env";
import axios from 'axios';
import DeviceStorage from "../services/DeviceStorage";

export default function LoginScreen() {
	const [email, setEmail] = useState("");
	const [password, setPassword] = useState("");
	const [password2, setPassword2] = useState("");
	const [signUpToggle, setSignUpToggle] = useState(false);
	const [forgotPasswordToggle, setForgotPasswordToggle] = useState(false);
	const [errors, setErrors] = useState({
		emailEmpty: false,
		emailInvalid: false,
		emailAlreadyExists: false,
		passwordEmpty: false,
		passwordInvalid: false,
		passwordMissmatch: false,
		unknownError: false,
		wrongCredentials: false
	})
	const [currentUser, setCurrentUser] = useState({});
	const signup = () => {
		if (!signUpToggle)
		{
			setSignUpToggle(true);
		}
		else
		{
			if (validateSignUpCredentials())
			{
				var user = new User();
				user.Login = email;
				user.Email = email;
				user.Password = password;

				const headers = {
					'Content-Type': 'application/json',
					//'Authorization': 'JWT fefege...'
				}
				axios.post(ApiUrl + "User/Register/", user, { headers: headers})
					.then((response) => {
						if (response.status == 200)
						{
							handleLogin(user);
						}
						else
						{
							errors.unknownError = true;
							setErrors({...errors});
						}
					})
					.catch((error) => {
						if (error.response.status == 409)
						{
							errors.emailAlreadyExists = true;
							setErrors({...errors});
						}
						else
						{
							errors.unknownError = true;
							setErrors({...errors});
						}
					});
			}
		}
	}
	const login = () => {
		if (validateLoginCredentials())
		{
			var user = new User();
			user.Login = email;
			user.Password = password;

			const headers = {
				'Content-Type': 'application/json',
				//'Authorization': 'JWT fefege...'
			}
			axios.post(ApiUrl + "User/Login/", user, { headers: headers})
				.then((response) => {
					if (response.status == 200)
					{
						handleLogin(user);
					}
					else
					{
						errors.unknownError = true;
						setErrors({...errors});
					}
				})
				.catch((error) => {
					if (error.response.status == 401)
					{
						errors.wrongCredentials = true;
						setErrors({...errors});
					}
					else
					{
						errors.unknownError = true;
						setErrors({...errors});
					}
				});
		}
	}
	const handleLogin = (user: object) => {
		DeviceStorage.saveItem("currentUser", user);
		setCurrentUser({...user});
		
	}
	const cancel = () => {
		setSignUpToggle(false);
		setForgotPasswordToggle(false);
	}
	const initErrors = () => {
		for (const [key, value] of Object.entries(errors))
		{
			Object.defineProperty(errors, key, { value: false });
		}
		setErrors({...errors});
	}
	const checkErrors = () => {
		setErrors({...errors});
		var ret = true;
		for (const [key, value] of Object.entries(errors))
		{
			ret = ret && !value;
		}
		return ret;
	}
	const validateSignUpCredentials = () => {
		initErrors();
		if (email.length == 0)
		{
			errors.emailEmpty = true;
		}
		else 
		{
			if (email.indexOf("@") < 1 || email.lastIndexOf(".") < email.indexOf("@") + 2)
			{
				errors.emailInvalid = true;
			}
		}
		if (password.length == 0)
		{
			errors.passwordEmpty = true;
		}
		else
		{
			var lowerCaseLetters = /[a-z]/g;
			var upperCaseLetters = /[A-Z]/g;
			var numbers = /[0-9]/g;
			var specialChars = /[( )!?@#$%^&*<>,;.:/\\]/g;
			if (password.length < 8 ||
				!password.match(lowerCaseLetters) ||
				!password.match(upperCaseLetters) ||
				!password.match(numbers) ||
				!password.match(specialChars))
			{
				errors.passwordInvalid = true;
			}
			if (password != password2)
			{
				errors.passwordMissmatch = true;
			}
		}
		return checkErrors();
	}
	const validateLoginCredentials = () => {
		initErrors();
		if (email.length == 0)
		{
			errors.emailEmpty = true;
		}
		if (password.length == 0)
		{
			errors.passwordEmpty = true;
		}
		return checkErrors();
	}
	const forgotPassword = () => {
		setForgotPasswordToggle(true);
	}
	const retrievePassword = () => {
		console.log("TODO: validate data and retrieve password");
	}

	return (
		<View style={styles.container}>
			<Image style={styles.image} source={require("../assets/company-logo.png")} />

			<View style={styles.inputContainer}>
				<View style={styles.inputView}>
					<TextInput
						style={styles.textInput}
						placeholder="Email"
						placeholderTextColor="#003f5c"
						onChangeText={(email) => setEmail(email)}
					/>
				</View>
				{
					errors.emailEmpty ?
					(
						<Text style={styles.error}>
							This field is required.
						</Text>
					) : null
				}
				{
					errors.emailInvalid ?
					(
						<Text style={styles.error}>
							This is not a valid email.
						</Text>
					) : null
				}
				{
					errors.emailAlreadyExists ?
					(
						<Text style={styles.error}>
							This email is already registered.
						</Text>
					) : null
				}
			</View>
			{
				!forgotPasswordToggle ?
				( 
					<View style={styles.inputContainer}>
						<View style={styles.inputView}>
							<TextInput
								style={styles.textInput}
								placeholder="Password"
								placeholderTextColor="#003f5c"
								secureTextEntry={true}
								onChangeText={(password) => setPassword(password)}
							/>
						</View>
						{
							errors.passwordEmpty ?
							(
								<Text style={styles.error}>
									This field is required.
								</Text>
							) : null
						}
						{
							errors.passwordInvalid ?
							(
								<Text style={styles.error}>
									{
										`The password needs to answer these criterias:\n` + 
										`\t- at least 8 characters.\n` + 
										`\t- at least one lower case character.\n` +
										`\t- at least one upper case character.\n` +
										`\t- at least one digit.\n` +
										`\t- at least one special character:\n\t\t\t\t( )!?@#$%^&*<>,;.:/\\`
									}
								</Text>
							) : null
						}
					</View>
				) : null
			}

			{ signUpToggle ?
				(
					<View style={styles.inputContainer}>
						<View style={styles.inputView}>
						<TextInput
							style={styles.textInput}
							placeholder="Retype Password"
							placeholderTextColor="#003f5c"
							secureTextEntry={true}
							onChangeText={(password2) => setPassword2(password2)}
						/>
						</View>
						{
							errors.passwordMissmatch ?
							(
								<Text style={styles.error}>
									The passwords are not matching.
								</Text>
							) : null
						}
					</View>
				) : null
			}

			{
				errors.wrongCredentials ?
				(
					<Text style={styles.error}>
						Your credentials are incorrect.
					</Text>
				) : null
			}

			{
				errors.unknownError ?
				(
					<Text style={styles.error}>
						Unknown error.
					</Text>
				) : null
			}

			{
				!signUpToggle && !forgotPasswordToggle ?
				(
					<TouchableOpacity onPress={forgotPassword}>
						<Text style={styles.forgot_button}>Forgot Password?</Text>
					</TouchableOpacity>
				) : null
			}
			
			{
				!signUpToggle && !forgotPasswordToggle ?
				(
				<TouchableOpacity style={styles.btnContainer} onPress={login}>
					<Text style={styles.button}>LOGIN</Text>
				</TouchableOpacity>
				) : null
			}

			{
				!forgotPasswordToggle ?
				(
					<TouchableOpacity style={[styles.btnContainer, signUpToggle ? styles.spacer : null]} onPress={signup}>
						<Text style={styles.button}>SIGN UP</Text>
					</TouchableOpacity>
				) : null
			}
			
			{
				forgotPasswordToggle ?
				(
				<TouchableOpacity style={styles.btnContainer} onPress={retrievePassword}>
					<Text style={styles.button}>SEND PASSWORD</Text>
				</TouchableOpacity>
				) : null
			}

			{
				signUpToggle || forgotPasswordToggle ?
				(
				<TouchableOpacity style={styles.btnContainer} onPress={cancel}>
					<Text style={styles.button}>CANCEL</Text>
				</TouchableOpacity>
				) : null
			}
		</View>
	);
}

const styles = StyleSheet.create({
	container: {
		width: "95%",
		height: "95%",
		margin: 10,
		alignItems: "center",
		justifyContent: "center"
	},
 
	image: {
		width: 355,
		height: 300,
		marginBottom: 20
	},

	inputContainer: {
		width: "80%",
		maxWidth: 500,
		alignItems: "center",
		marginTop: 10,
		marginBottom: 10
	},
 
	inputView: {
		width: "100%",
		backgroundColor: "#c0d8ff",
		borderRadius: 20,
		alignItems: "center",
	},
 
	textInput: {
		width: "90%",
		height: 40,
		...Platform.select({
			web: {
				outlineStyle: "none"
			}
		})
	},
 
	forgot_button: {
		marginTop: 10,
		marginBottom: 30
	},
 
	btnContainer: {
		width: "40%",
		maxWidth: 250,
		borderRadius: 20,
		marginTop: 10,
		marginBottom: 10,
		height: 40
	},

	button: {
		width: "100%",
		backgroundColor: "#458cff",
		borderRadius: 20,
		flex: 1,
		textAlign: "center",
		alignItems: "center",
		lineHeight: 40
	},

	spacer: {
		marginTop: 30
	},

	error: {
		color: "#ff0000"
	}
});