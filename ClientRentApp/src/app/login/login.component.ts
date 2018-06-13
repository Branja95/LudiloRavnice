import { Component, OnInit } from '@angular/core';

import { NgForm } from '@angular/forms';

import { LoginUser } from '../models/login-user.model';

import { LoginService } from '../services/login.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  providers: [LoginService]
})
export class LoginComponent implements OnInit {

  constructor(private loginService:LoginService) { }

  ngOnInit() {
  }

  onSubmit(form: NgForm, loginUser: LoginUser) {
    console.log(loginUser);
    this.loginService.postMethodLogin(loginUser)
  }

}
