import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import {
  Router,
  ActivatedRoute
} from '@angular/router';

import { BranchOffice } from '../models/branch-office.model';
import { BranchOfficeService } from '../services/branch-office.service';

@Component({
  selector: 'app-edit-branch-office',
  templateUrl: './edit-branch-office.component.html',
  styleUrls: ['./edit-branch-office.component.css']
})
export class EditBranchOfficeComponent implements OnInit  {
  BranchOfficeId: string = "-1";
  url: string = '';
  file: File = null;
  BranchOffice: BranchOffice;
  
  constructor(private router: Router, private activatedRoute: ActivatedRoute, private branchOfficeService: BranchOfficeService) {
    activatedRoute.params.subscribe(params => {this.BranchOfficeId = params["BranchOfficeId"]});
  }

  ngOnInit() {
    this.branchOfficeService.getBranchOffice(this.BranchOfficeId).subscribe(
      data => {
        this.BranchOffice = data as BranchOffice;
      },error => {
        alert(error.error.Message);
      });
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

  onSubmit(form: NgForm, branchOffice: BranchOffice) {

    this.branchOfficeService.editBranchOffice(this.BranchOfficeId, branchOffice)
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
