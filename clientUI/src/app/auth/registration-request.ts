export interface RegistrationRequest {
    readonly firstName: string;
    readonly lastName: string;
    readonly email: string;
    readonly password: string;
    readonly confirmPassword: string;
}
