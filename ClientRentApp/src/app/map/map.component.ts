import { Component, OnInit, Output, Input, EventEmitter } from '@angular/core';
import { Router, ActivatedRoute} from '@angular/router';
import { BranchOfficeService } from '../services/branch-office.service';
import { MapInfo } from '../models/map-info.model';
import { BranchOffice } from '../models/branch-office.model';

@Component({
  selector: 'app-map',
  templateUrl: './map.component.html',
  styleUrls: ['./map.component.css'],
  styles: ['agm-map {height: 300px; width: 500px;}'] //postavljamo sirinu i visinu mape
})

export class MapComponent implements OnInit {
 
  @Input() branchOfficeId: string;
 
  @Output() messageEvent = new EventEmitter<MapInfo>();

  mapInfo: MapInfo;
  
  branchOffice: any;

  ngOnInit() {
    this.branchOffice = this.getBranchOffice();
    this.mapInfo = new MapInfo(this.branchOffice.Latitude, this.branchOffice.Longitude, "assets/ftn.png", "ftn" , "" , "http://ftn.uns.ac.rs/691618389/fakultet-tehnickih-nauka");
  }

  placeMarker($event){
    this.mapInfo.centerLat = $event.coords.lat;
    this.mapInfo.centerLong = $event.coords.lng;

    this.sendMessage();
  }
  
  constructor(private router: Router, private activatedRoute: ActivatedRoute, private branchOfficeService: BranchOfficeService){ }

  getBranchOffice(){
    this.branchOfficeService.getBranchOffice(this.branchOfficeId).subscribe(
      res => {
        this.branchOffice = res as any;
        this.mapInfo = new MapInfo(this.branchOffice.Latitude, this.branchOffice.Longitude, 
          "assets/ftn.png",
          this.branchOffice.Address , "" , "");
      }, error =>{
        console.log(error);
      })
  }

  sendMessage() {
    this.messageEvent.emit(this.mapInfo)
  }

}