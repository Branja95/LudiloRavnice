import { Component, OnInit } from '@angular/core';

import { RegistrationService } from '../services/registration.service';

import { NgForm } from '@angular/forms';

@Component({
  selector: 'app-approve-account',
  templateUrl: './approve-account.component.html',
  styleUrls: ['./approve-account.component.css'],
  providers: [RegistrationService]
})
export class ApproveAccountComponent implements OnInit {

  url: string = '';
  file: File = null;

  constructor(private registrationService: RegistrationService) { }

  ngOnInit() {
  }

  handleFileInput(event) {
    this.file = event.target.files[0];
    
    if (event.target.files && event.target.files[0]) {
      var reader = new FileReader();

      reader.readAsDataURL(event.target.files[0]); 

      reader.onload = (event) => { 
        this.url = reader.result;
      }
    }
  }

  onSubmit(form: NgForm) {
    if(this.file != null)
    {
      this.registrationService.postMethodApproveAccount(this.file)
      .subscribe(
        data => {
          alert(data);
        }, error => {
          alert(error.error.Message);
        });;

      form.reset();
      this.url = '';
      this.file = null;
    }
  }

}
