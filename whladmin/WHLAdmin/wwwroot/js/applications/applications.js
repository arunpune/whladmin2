$(function () {

  $('.whl-action-delete-application').on('click', function (e) {
    e.preventDefault();
    var applicationId = $(e.currentTarget).data('applicationid');
    var applicationName = $(e.currentTarget).data('applicationname');
    if (confirm('Are you sure you want to delete application - ' + applicationName + '?')) {
      $.post(applicationDeleteUrl + '?applicationId=' + applicationId)
        .done(function (response) {
          window.location.reload();
        });
    }
  });

  $('.whl-action-download-results').on('click', function (e) {
    e.preventDefault();
    const listingId = $('.whl-formfield-listingid').val();
    const submissionTypeCd = $('.whl-formfield-submissiontypecd').val();
    const statusCd = $('.whl-formfield-statuscd').val();
    window.open(applicationsUrl + '?listingId=' + listingId + '&submissionTypeCd=' + submissionTypeCd + '&statusCd=' + statusCd + '&download=Y', '_whladminDownload');
  });

  $('.whl-action-gotopage').on('click', function (e) {
    $('.whl-formfield-pageno').val($(this).data('pageno'));
    $('.whl-action-filter-applications').trigger('click');
  });

});