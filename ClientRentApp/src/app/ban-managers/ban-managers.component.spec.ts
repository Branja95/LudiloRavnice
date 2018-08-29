import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BanManagersComponent } from './ban-managers.component';

describe('BanManagersComponent', () => {
  let component: BanManagersComponent;
  let fixture: ComponentFixture<BanManagersComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BanManagersComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BanManagersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
