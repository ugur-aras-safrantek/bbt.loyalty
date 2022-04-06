interface IColumn {
  columnName: string;
  propertyName: string;
  isBoolean: boolean
}

export class Column implements IColumn {
  columnName: string;
  propertyName: string;
  isBoolean: boolean;
}
