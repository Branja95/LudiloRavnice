import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { BranchOffice } from '../models/branch-office.model';
import { BranchOfficeService } from '../services/branch-office.service';

@Component({
  selector: 'app-branch-office',
  templateUrl: './branch-office.component.html',
  styleUrls: ['./branch-office.component.css'],
  providers: [BranchOfficeService]
})

export class BranchOfficeComponent implements OnInit {
  
  url: string = '';
  file: File = null;
  serviceId : string = "-1";
  branchOffices = Array<BranchOffice>()

  constructor(private router: Router, private activatedRoute: ActivatedRoute, private branchOfficeService: BranchOfficeService) {
    activatedRoute.params
    .subscribe(params => {
      this.serviceId = params["ServiceId"];
    });
  }

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

  onDelete(branchOfficeId: string){
    this.branchOfficeService.deleteBranchOffice(this.serviceId, branchOfficeId)
    .subscribe(
      res => {
        console.log(res);
      },
      error => {
        alert(error);
      })
  }

  onEdit(branchOfficeId)
  {
    this.router.navigate(['/EditBranchOffice', this.serviceId, branchOfficeId]);
  }

  isManagerOrAdmin(){

    if(!localStorage.role)
    {
      return false;
    }
    else
    {
      if(localStorage.role == "Manager" || localStorage.role == "Admin")
      {
        return true;
      }
      return false;
    }
  }


  getBranchOffices() { 
    this.branchOfficeService.getBranchOffices(this.serviceId)
    .subscribe(
      res => { 
          this.branchOffices = res as Array<BranchOffice>;
      }, 
      error => {
        alert(error);
      });
  }

}
