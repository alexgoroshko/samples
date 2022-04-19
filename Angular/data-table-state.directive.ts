import { AfterViewInit, ChangeDetectorRef, Directive, OnDestroy, ViewChild } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { ActivatedRoute, Router } from '@angular/router';
import { merge, MonoTypeOperatorFunction, Observable, of, OperatorFunction, Subject } from 'rxjs';
import { catchError, finalize, startWith, switchMap, takeUntil, tap } from 'rxjs/operators';
import { FilterModel } from '../common/models/filter.model';
import { SnackbarService } from '../common/services/snackbar.service';
import { TablePageDataModel } from '../common/models/table-page-data.model';
import { ExportService } from '../modules/shared/export-button/export.service';
import { FormGroup } from '@angular/forms';
import { ITableDataService } from '../common/interfaces/table-data.service';

@Directive()
export abstract class DataTableStateDirective<T> implements AfterViewInit, OnDestroy {
  get searchString() {
    return this.filter?.searchString;
  }

  set searchString(value) {
    this.filter = {
      ...this.filter,
      searchString: value
    };
  }

  get searchFieldName() {
    return this.filter?.searchFieldName;
  }

  set searchFieldName(value) {
    this.filter = {
      ...this.filter,
      searchFieldName: value
    };
  }

  get searchKind() {
    return this.filter?.searchKind;
  }

  set searchKind(value) {
    this.filter = {
      ...this.filter,
      searchKind: value
    };
  }

  get filter(): FilterModel | undefined {
    return this._filter;
  }

  set filter(value: FilterModel | undefined) {
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: value,
      queryParamsHandling: null
    });

    this._filter = value;
  }

  get csvFilter(): FilterModel {
    return {
      ...this.filter,
      pageSize: this.count
    };
  }

  get pageSize(): number {
    return this.filter?.pageSize || this.defaultPageSize;
  }

  @ViewChild(MatPaginator) paginator: MatPaginator | undefined;
  @ViewChild(MatSort) sort: MatSort | undefined;

  public dataSource: T[] = [];
  public count = 0;
  public loading = false;
  public exporting = false;
  public sortOrPaginationChange$: Observable<any> | undefined;
  public filterGroup: FormGroup = new FormGroup({});
  public readonly pageSizeOptions = [5, 10, 25, 50, 100, 200];
  public readonly defaultPageSize: number = 200;

  protected destroy$ = new Subject();

  private _filter: FilterModel | undefined;

  protected constructor(
    public route: ActivatedRoute,
    public router: Router,
    public snackbarService: SnackbarService,
    public cdr: ChangeDetectorRef,
    public exportService?: ExportService
  ) {}

  ngAfterViewInit() {
    this.route.queryParams.subscribe((res: FilterModel) => {
      this.filter = res;
    });

    if (this.sort && this.paginator) {
      this.sortOrPaginationChange$ = merge(this.sort.sortChange, this.paginator.page).pipe(startWith(this.filter), this._mapFilter());
    }

    if (this.sort && !this.paginator) {
      this.sortOrPaginationChange$ = this.sort.sortChange.pipe(startWith(this.filter), this._mapFilter());
    }

    if (this.paginator && !this.sort) {
      this.sortOrPaginationChange$ = this.paginator.page.pipe(startWith(this.filter), this._mapFilter());
    }

    if (this.sortOrPaginationChange$) {
      this.sortOrPaginationChange$
        .pipe(
          switchMap(() => this._fetchData(false)),
          takeUntil(this.destroy$)
        )
        .subscribe();
    }
  }

  ngOnDestroy() {
    this.destroy$.next(true);
    this.destroy$.complete();
  }

  public onSearch($event: string) {
    this.searchString = $event;
    this.fetchFirstPage();
  }

  public resetFilter() {
    this.filterGroup.reset();
    this.fetchFirstPage();
  }

  public applyFilter() {
    this.fetchFirstPage();
  }

  public fetchFirstPage() {
    if (this.paginator?.pageIndex !== 0) {
      this.paginator?.firstPage();
    } else {
      this._fetchData(false).subscribe();
    }
  }

  public baseExport(csv: boolean, mapFunc: OperatorFunction<TablePageDataModel<T>, T[]>) {
    this.exporting = true;

    this._fetchData(true)
      .pipe(
        mapFunc,
        switchMap(async (items) => (csv ? this.exportService?.exportCSVFile(items) : this.exportService?.exportXLSXFile(items))),
        takeUntil(this.destroy$)
      )
      .subscribe({
        next: () => {
          this.exporting = false;

          this.snackbarService.showSuccessSnackbar();
          this.cdr.markForCheck();
        },
        error: (err) => {
          this.snackbarService.showErrorSnackbar();
          this.exporting = false;
          this.cdr.markForCheck();

          throw err;
        }
      });
  }

  public baseFetchData<R extends TablePageDataModel<T> = TablePageDataModel<T>, F extends FilterModel = FilterModel>(
    isCsv: boolean,
    tableDataService: ITableDataService<R, F>
  ): Observable<R> {
    this.loading = true;

    return tableDataService.fetchTableData((isCsv ? this.csvFilter : this.filter) as F).pipe(
      tap((response) => {
        if (!isCsv) {
          this.count = response.count;
          this.dataSource = response.items;
        }
      }),
      finalize(() => {
        this.loading = false;
        this.cdr.markForCheck();
      }),
      catchError((err) => {
        this.snackbarService.showErrorSnackbar();
        throw Error(err.error);
      }),
      takeUntil(this.destroy$)
    );
  }

  private _mapFilter(): MonoTypeOperatorFunction<any> {
    return tap((res: FilterModel) => {
      if (this.paginator && res?.pageIndex) {
        this.paginator.pageIndex = res.pageIndex;
      }

      if (this.paginator && !res.skip) {
        this.filter = {
          pageSize: this.paginator.pageSize,
          skip: this.paginator?.pageIndex * this.paginator?.pageSize,
          sortByFieldName: this.sort?.active,
          searchFieldName: this.filter?.searchFieldName,
          sortOrder: this.sort?.direction,
          searchString: this.filter?.searchString,
          pageIndex: this.paginator?.pageIndex
        };
      }
    });
  }

  protected abstract _fetchData(isCsv: boolean): Observable<any>;
}
