import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';

import { User } from '../models/user.model';

import { RegistrationService } from '../services/registration.service';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.css'],
  providers: [RegistrationService]
})
export class RegistrationComponent implements OnInit {

  constructor(private registrationService: RegistrationService) { }

  ngOnInit() {
  }

  onSubmit(form: NgForm, user: User) {
    console.log(user);
    this.registrationService.postMethodRegistration(user)
    .subscribe(
      data => {
        alert(data);
      },
      error => {
        console.log(error);
      })
  }

}
