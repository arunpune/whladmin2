$(function () {

  $('.whl-action-edit-amenities').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    const listingId = $(e.currentTarget).data('listingid');
    $.get(listingAmenitiesEditUrl + '?listingId=' + listingId)
      .done(function (response) {
        $('#whl-title-amenity-action').html(amenitiesCount > 0 ? 'Edit Amenities' : 'Add Amenities');
        $('#hidAmenityAction').val('EDIT');
        $('#amenityEditorModal').find('div.modal-body').html(response);
        $('#amenityEditorModal').modal('show');
        $('#lblAmenitySaveErrorMessage').html('');
        $('#divAmenitySaveErrorMessage').hide();
      });
  });

  $('.whl-action-save-amenity').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('#lblAmenitySaveErrorMessage').html('');
    $('#divAmenitySaveErrorMessage').hide();

    let selectedAmenityIds = '';
    $("input:checkbox[name=selAmenities]:checked").each(function () {
      selectedAmenityIds += (selectedAmenityIds.length > 0 ? ',' : '') + $(this).val();
    });

    const a = $('#hidAmenityAction').val();
    const url = a === 'EDIT' ? listingAmenitiesEditUrl : listingAmenitiesAddUrl;
    const data = {
      ListingId: $('#AmenityListingId').val(),
      AmenityIds: selectedAmenityIds
    };
    $.post(url, data)
      .done(function (response) {
        $('#amenityEditorModal').find('div.modal-body').html('');
        $('#amenityEditorModal').modal('hide');
        const listingId = $('#PageListingId').val();
        fnShowToast('SUCCESS', 'Amenities for Listing #' + listingId + ' updated, refreshing page.');
      })
      .fail(function (e) {
        const message = e.responseJSON && e.responseJSON.message && e.responseJSON.message.length > 0 ? e.responseJSON.message : 'Failed to save amenities';
        $('#lblAmenitySaveErrorMessage').html(e.responseJSON.message);
        $('#divAmenitySaveErrorMessage').show();
      });
  });

});