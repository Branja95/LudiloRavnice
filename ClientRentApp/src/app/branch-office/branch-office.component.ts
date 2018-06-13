import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';

import { BranchOffice } from '../models/branch-office.model';

import { BranchOfficeService } from '../services/branch-office.service';

@Component({
  selector: 'app-branch-office',
  templateUrl: './branch-office.component.html',
  styleUrls: ['./branch-office.component.css'],
  providers: [BranchOfficeService]
})
export class BranchOfficeComponent implements OnInit {

  constructor(private branchOfficeService: BranchOfficeService) { }

  ngOnInit() {
  }

  onSubmit(form: NgForm, branchOffice: BranchOffice) {
    console.log(branchOffice);
    this.branchOfficeService.postMethodCreateBranchOffice(branchOffice)
    .subscribe(
      data => {
        alert(data);
      },
      error => {
        alert(error.error.Message);
      })
  }

}
