import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewRentVehicleComponent } from './view-rent-vehicle.component';

describe('ViewRentVehicleComponent', () => {
  let component: ViewRentVehicleComponent;
  let fixture: ComponentFixture<ViewRentVehicleComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ViewRentVehicleComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ViewRentVehicleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
