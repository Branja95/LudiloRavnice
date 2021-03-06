import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { User } from '../models/user.model';
import { Router } from '@angular/router';
import { RegistrationService } from '../services/registration.service';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.css'],
  providers: [RegistrationService]
})
export class RegistrationComponent implements OnInit {

  constructor(private registrationService: RegistrationService, private router: Router) { }

  ngOnInit() {
  }

  onSubmit(form: NgForm, user: User) {
    this.registrationService.postMethodRegistration(user).subscribe(
      res => {
        console.log(res);
        this.router.navigate(['/Login']);
      },
      error => {
        alert(error.error.Message);
    });
  }

}
