$(function () {

  $('.whl-action-add-image').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    const listingId = $(e.currentTarget).data('listingid');
    $.get(listingImageAddUrl + '?listingId=' + listingId)
      .done(function (response) {
        $('#whl-title-image-action').html('Add Image');
        $('#hidImageAction').val('ADD');
        $('#imageEditorModal').find('div.modal-body').html(response);
        $('#imageEditorModal').modal('show');
        $('#lblImageSaveErrorMessage').html('');
        $('#divImageSaveErrorMessage').hide();
        $('#imageFile').on('change', function (e) {
          if (fnIsValidFileSize('imageFile', 1)) {
            fnGetBase64('imageFile', 'Contents');
          }
        });
        $('#thumbnailFile').on('change', function (e) {
          if (fnIsValidFileSize('imageFile', 1)) {
            fnGetBase64('thumbnailFile', 'ThumbnailContents');
          }
        });
      });
  });

  $('.whl-action-edit-image').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    const imageId = $(e.currentTarget).data('imageid');
    $.get(listingImageEditUrl + '?imageId=' + imageId)
      .done(function (response) {
        $('#whl-title-image-action').html('Edit Image Properties');
        $('#hidImageAction').val('EDIT');
        $('#imageEditorModal').find('div.modal-body').html(response);
        $('#imageEditorModal').modal('show');
        $('#lblImageSaveErrorMessage').html('');
        $('#divImageSaveErrorMessage').hide();
      });
  });

  $('.whl-action-save-image').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    $('#lblImageSaveErrorMessage').html('');
    $('#divImageSaveErrorMessage').hide();
    const a = $('#hidImageAction').val();
    const url = a === 'EDIT' ? listingImageEditUrl : listingImageAddUrl;
    const data = {
      ImageId: $('#ImageId').val(),
      ListingId: $('#ImageListingId').val(),
      Title: $('#Title').val(),
      Contents: $('#Contents').val(),
      ThumbnailContents: $('#ThumbnailContents').val(),
      MimeType: $('#MimeType').val(),
      IsPrimary: $('#IsPrimary').prop('checked'),
      DisplayOnListingsPageInd: $('#DisplayOnListingsPageInd').prop('checked')
    };
    $.post(url, data)
      .done(function (response) {
        $('#imageEditorModal').find('div.modal-body').html('');
        $('#imageEditorModal').modal('hide');
        const listingId = $('#PageListingId').val();
        fnShowToast('SUCCESS', 'Images for Listing #' + listingId + ' updated, refreshing page.');
      })
      .fail(function (e) {
        const message = e.responseJSON && e.responseJSON.message && e.responseJSON.message.length > 0 ? e.responseJSON.message : 'Failed to save image';
        $('#lblImageSaveErrorMessage').html(e.responseJSON.message);
        $('#divImageSaveErrorMessage').show();
      });
  });

  $('.whl-action-delete-image').on('click', function (e) {
    e.preventDefault();
    e.stopPropagation();
    const imageId = $(e.currentTarget).data('imageid');
    if (confirm('Are you sure you want to delete listing image - ' + imageId + '?')) {
      $.post(listingImageDeleteUrl + '?imageId=' + imageId)
        .done(function (response) {
          const listingId = $('#PageListingId').val();
          fnShowToast('SUCCESS', 'Image ' + imageId + ' for Listing #' + listingId + ' deleted, refreshing page.');
        });
    }
  });

});