$(function () {

  $('.whl-formfield-salesviewtypecd').on('change', function (e) {
    $('#pageViewTypeCd').val('SALES');
    $('#salesViewTypeCd').val($(e.currentTarget).val());
    $('.whl-listings-search-submit').click();
  });

  $('.whl-formfield-rentalsviewtypecd').on('change', function (e) {
    $('#rentalsViewTypeCd').val($(e.currentTarget).val());
    $('#pageViewTypeCd').val('RENTALS');
    $('.whl-listings-search-submit').click();
  });

  $('.whl-listings-show-filter').hide();
  $('.whl-listings-current-filters').hide();
  $('.whl-listings-search-container').show();

  $('.whl-listings-show-filter').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('.whl-listings-current-filters').hide();
    $('.whl-listings-search-container').show();
  });

  //$('.whl-form-listings-search').on('submit', function (e) {
  //  e.preventDefault();
  //  e.stopPropagation();
  //  console.log('formsubmission');
  //  $('.whl-listings-show-filter').show();
  //  $('.whl-listings-search-container').hide();
  //});

  $('.whl-listings-search-close').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('.whl-listings-current-filters').show();
    $('.whl-listings-search-container').hide();
  });

});