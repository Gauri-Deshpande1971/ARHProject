import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ButtonRendererComponentComponent } from './button-renderer-component.component';

describe('ButtonRendererComponentComponent', () => {
  let component: ButtonRendererComponentComponent;
  let fixture: ComponentFixture<ButtonRendererComponentComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ButtonRendererComponentComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ButtonRendererComponentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
