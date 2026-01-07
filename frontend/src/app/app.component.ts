import { Component } from '@angular/core';
import { LoansListComponent } from './loans-list/loans-list.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [LoansListComponent],
  template: `<main><app-loans-list></app-loans-list></main>`,
})
export class AppComponent {}
