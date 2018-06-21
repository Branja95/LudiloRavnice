import { Component, OnInit } from '@angular/core';

import { NgForm } from '@angular/forms';

import { Router, ActivatedRoute } from '@angular/router';

import { LoginUser } from '../models/login-user.model';

import { LoginService } from '../services/login.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  providers: [LoginService]
})
export class LoginComponent implements OnInit {

  constructor(private loginService:LoginService, private router: Router) { }

  ngOnInit() {
  }

  onSubmit(form: NgForm, loginUser: LoginUser) {
    if(!localStorage.jwt)
    {
      this.loginService.postMethodLogin(loginUser).subscribe(
        res => {
          this.saveToken(res);
          this.goToDefault();
        },
        err => {
          alert("Invalid username or/and password.")
        }
      );
    }
  }

  saveToken(data) : void {

    //console.log(res.access_token);

    let jwt = data.access_token;

    let jwtData = jwt.split('.')[1]

    let decodedJwtJsonData = window.atob(jwtData)
    let decodedJwtData = JSON.parse(decodedJwtJsonData)

    let role = decodedJwtData.role
    let uniqueName = decodedJwtData.unique_name

    console.log(decodedJwtData)
    //console.log('jwtData: ' + jwtData)
    //console.log('decodedJwtJsonData: ' + decodedJwtJsonData)
    //console.log('decodedJwtData: ' + decodedJwtData)
    //console.log('Role ' + role)

    localStorage.setItem('jwt', jwt)
    localStorage.setItem('role', role);
    localStorage.setItem('username', uniqueName);
  
  }

  goToDefault() : void {
    this.router.navigate(['/RentVehicle']);
  }
}
