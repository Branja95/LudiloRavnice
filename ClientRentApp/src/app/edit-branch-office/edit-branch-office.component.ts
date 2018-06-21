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
  styleUrls: ['./edit-branch-office.component.css']
})
export class EditBranchOfficeComponent implements OnInit  {
  
  BranchOfficeId: string = "-1";
  BranchOffice: BranchOffice;
    
  selecetdFileUrl: string = '';
  selectedFile: File = null;

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
  
    this.branchOfficeService.editBranchOffice(this.BranchOfficeId,this.BranchOffice, this.selectedFile)
    .subscribe(
      data => {
        alert(data);
      }, error => {
        alert(error);
      });;
    
      form.reset();
      this.selecetdFileUrl = '';
      this.selectedFile = null;
      this.router.navigateByUrl('BranchOffice');
    
  }
}
