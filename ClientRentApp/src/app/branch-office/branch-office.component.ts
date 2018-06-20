import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ActivatedRoute, Params } from '@angular/router';

import { BranchOffice } from '../models/branch-office.model';

import { BranchOfficeService } from '../services/branch-office.service';
import { Observable } from 'rxjs/Observable';
import { debug } from 'util';

@Component({
  selector: 'app-branch-office',
  templateUrl: './branch-office.component.html',
  styleUrls: ['./branch-office.component.css'],
  providers: [BranchOfficeService]
})
export class BranchOfficeComponent implements OnInit {

  url: string = '';
  file: File = null;

  branchOffices = Array<BranchOffice>()

  constructor(private branchOfficeService: BranchOfficeService) { }

  ngOnInit() {
    this.getBranchOffices();
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

    this.branchOfficeService.postMethodCreateBranchOffice(branchOffice, this.file)
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

  onDelete(form: NgForm, id: string){
    this.branchOfficeService.deleteBranchOffice(id)
    .subscribe(
      data => {
        alert(data);
      },
      error => {
        alert(error);
      }
    )
    
  }

  getBranchOffices() { 
    
    this.branchOfficeService.getMethodBranchOffices()
    .subscribe(
      res => { 
          this.branchOffices = res as Array<BranchOffice>;
      }, error => {
        alert(error);
      });
  }

}
