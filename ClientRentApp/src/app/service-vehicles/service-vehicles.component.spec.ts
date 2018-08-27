import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ServiceVehiclesComponent } from './service-vehicles.component';

describe('ServiceVehiclesComponent', () => {
  let component: ServiceVehiclesComponent;
  let fixture: ComponentFixture<ServiceVehiclesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ServiceVehiclesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ServiceVehiclesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
