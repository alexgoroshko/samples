import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CompaniesComponent } from './companies.component';
import { CreateEditCompanyComponent } from './create-edit-company/create-edit-company.component';

const routes: Routes = [
  { path: '', pathMatch: 'full', component: CompaniesComponent, data: { title: 'Menu.Companies' } },
  { path: 'edit/:id', pathMatch: 'full', component: CreateEditCompanyComponent, data: { title: 'Companies.UpdateCompany' } },
  { path: 'create', pathMatch: 'full', component: CreateEditCompanyComponent, data: { title: 'Companies.CreateCompany' } }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class CompaniesRoutingModule {}
