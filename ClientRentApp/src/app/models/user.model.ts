export class User {
    email: string;
    password: string;
    confirmedPassword: string;
    firstName: string;
    lastName: string;
    image: string;
    dateOfBirth: string;

    constructor(email: string, password: string, confirmedPassword: string, firstName: string, lastName: string, image: string, dateOfBirth: string) {
        this.email = email;
        this.password = password;
        this.confirmedPassword = confirmedPassword;
        this.firstName = firstName;
        this.lastName = lastName;
        this.image = image;
        this.dateOfBirth = dateOfBirth;
    }
}