import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BooleanRenderComponent } from './boolean-render.component';

describe('BooleanRenderComponent', () => {
  let component: BooleanRenderComponent;
  let fixture: ComponentFixture<BooleanRenderComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BooleanRenderComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BooleanRenderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
