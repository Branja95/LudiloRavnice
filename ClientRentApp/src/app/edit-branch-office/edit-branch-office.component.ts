import { Component, OnInit } from '@angular/core';
import { NgForm, FormsModule } from '@angular/forms';
import {
  Router,
  ActivatedRoute
} from '@angular/router';

import { BranchOffice } from '../models/branch-office.model';
import { BranchOfficeService } from '../services/branch-office.service';


@Component({
  selector: 'app-edit-branch-office',
  templateUrl: './edit-branch-office.component.html',
  styleUrls: ['./edit-branch-office.component.css'],
  providers: [BranchOfficeService]
})
export class EditBranchOfficeComponent implements OnInit  {
  
  ServiceId : string = "-1";
  BranchOfficeId: string = "-1";
  BranchOffice: BranchOffice;
    
  selecetdFileUrl: string = '';
  selectedFile: File = null;

  constructor(private router: Router, private activatedRoute: ActivatedRoute, private branchOfficeService: BranchOfficeService) {
    activatedRoute.params
    .subscribe(params => {
      this.ServiceId = params["ServiceId"];
      this.BranchOfficeId = params["BranchOfficeId"];
    });
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

    this.selectedFile = event.target.files[0];
    
    if (event.target.files && event.target.files[0]) {
      var reader = new FileReader();

      reader.readAsDataURL(event.target.files[0]); 

      reader.onload = (event) => { 
        this.selecetdFileUrl = reader.result;
      }
    }

  }

  onSubmit(form: NgForm) {
  
    this.branchOfficeService.editBranchOffice(this.ServiceId, this.BranchOfficeId,this.BranchOffice, this.selectedFile)
    .subscribe(
      res => {
        console.log(res);
      }, error => {
        alert(error.error.Message);
      });;
    
      form.reset();
      this.selecetdFileUrl = '';
      this.selectedFile = null;
    
  }
}
