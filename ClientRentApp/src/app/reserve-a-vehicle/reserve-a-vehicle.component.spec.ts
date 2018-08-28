import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ReserveAVehicleComponent } from './reserve-a-vehicle.component';

describe('ReserveAVehicleComponent', () => {
  let component: ReserveAVehicleComponent;
  let fixture: ComponentFixture<ReserveAVehicleComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ReserveAVehicleComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ReserveAVehicleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
